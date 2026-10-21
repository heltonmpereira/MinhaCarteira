using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.Servico.Helper;

/// <summary>
/// Classificação da notificação bancária para decisão de importação.
/// </summary>
public enum TipoNotificacaoBancaria
{
    Desconhecida,
    CompraAprovada,
    PagamentoRealizado,
    CompraRecusada,
    Promocao
}

public static class NotificacaoBancariaParser
{
    private const int TamanhoMaximoDescricao = 200;

    // Padrões de valor monetário, do mais específico para o mais genérico.
    private static readonly Regex[] PadroesValor =
    [
        // Formato brasileiro: R$ 1.375,00 / R$ 159,90
        new(@"R\$\s*(\d{1,3}(?:\.\d{3})*,\d{2})", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"R\$\s*(\d+,\d{2})", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        // Formato com ponto decimal: R$ 90.00
        new(@"R\$\s*(\d{1,3}(?:\.\d{3})*\.\d{2})", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        // Formato brasileiro sem centavos: R$ 1.375 / R$ 90
        new(@"R\$\s*(\d{1,3}(?:\.\d{3})+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"R\$\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        // Sem o símbolo R$ (fallback).
        new(@"(\d{1,3}(?:\.\d{3})+,\d{2})", RegexOptions.Compiled),
        new(@"(\d+,\d{2})", RegexOptions.Compiled),
        new(@"(\d+\.\d{2})", RegexOptions.Compiled)
    ];

    // Data no formato dd/MM/yyyy, com horário opcional (ex.: "às 05:14").
    private static readonly Regex PadraoData = new(
        @"(\d{1,2})/(\d{1,2})/(\d{2,4})(?:\s*(?:às|as)\s*(\d{1,2}):(\d{2}))?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Horário isolado (ex.: "09:17").
    private static readonly Regex PadraoHora = new(@"(\d{1,2}):(\d{2})", RegexOptions.Compiled);

    // Estabelecimento em compras: o texto entre ", em " e ", foi".
    private static readonly Regex PadraoDescricaoCompra = new(
        @"\bem\s+([^,]+?)\s*,\s*foi",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Estabelecimento em pagamentos: o texto entre "para " e " foi realizado".
    private static readonly Regex PadraoDescricaoPagamento = new(
        @"\bpara\s+(.+?)\s+foi\s+realizado",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Dictionary<string, DayOfWeek> DiasSemana = new(StringComparer.OrdinalIgnoreCase)
    {
        ["domingo"] = DayOfWeek.Sunday,
        ["segunda-feira"] = DayOfWeek.Monday,
        ["segunda"] = DayOfWeek.Monday,
        ["terca-feira"] = DayOfWeek.Tuesday,
        ["terca"] = DayOfWeek.Tuesday,
        ["quarta-feira"] = DayOfWeek.Wednesday,
        ["quarta"] = DayOfWeek.Wednesday,
        ["quinta-feira"] = DayOfWeek.Thursday,
        ["quinta"] = DayOfWeek.Thursday,
        ["sexta-feira"] = DayOfWeek.Friday,
        ["sexta"] = DayOfWeek.Friday,
        ["sabado"] = DayOfWeek.Saturday
    };

    private static readonly string[] TermosRecusada =
    [
        "recusad", "negad", "nao autorizad", "bloquead", "cancelad", "expirad"
    ];

    private static readonly string[] TermosPromocao =
    [
        "oferta", "cupom", "promo", "desconto", "clube", "aproveite"
    ];

    private static readonly string[] TermosCredito =
    [
        "receb", "credit", "credito", "entrada", "deposit", "estorno",
        "reembolso", "resgate", "rendimento", "salario", "pix", "transferencia"
    ];

    /// <summary>
    /// Classifica a notificação para decidir se ela representa uma operação efetuada.
    /// </summary>
    public static TipoNotificacaoBancaria Classificar(string texto)
    {
        var normalizado = RemoverAcentos(texto)?.ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizado))
            return TipoNotificacaoBancaria.Desconhecida;

        // Recusadas/negadas têm precedência (ex.: "compra ... foi recusada").
        if (TermosRecusada.Any(termo => normalizado.Contains(termo)))
            return TipoNotificacaoBancaria.CompraRecusada;

        if (TermosPromocao.Any(termo => normalizado.Contains(termo)))
            return TipoNotificacaoBancaria.Promocao;

        if (normalizado.Contains("aprovad"))
            return TipoNotificacaoBancaria.CompraAprovada;

        if (normalizado.Contains("pago") || normalizado.Contains("pagamento"))
            return TipoNotificacaoBancaria.PagamentoRealizado;

        return TipoNotificacaoBancaria.Desconhecida;
    }

    /// <summary>
    /// Indica se a notificação deve gerar um movimento bancário.
    /// </summary>
    public static bool EhImportavel(TipoNotificacaoBancaria tipo) => tipo is 
            TipoNotificacaoBancaria.CompraAprovada 
         or TipoNotificacaoBancaria.PagamentoRealizado 
         or TipoNotificacaoBancaria.Desconhecida;

    /// <summary>
    /// Extrai o valor monetário informado no texto da notificação.
    /// Retorna 0 quando nenhum valor válido é encontrado.
    /// </summary>
    public static decimal ExtrairValor(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return 0;

        foreach (var padrao in PadroesValor)
        {
            var match = padrao.Match(texto);
            if (!match.Success)
                continue;

            var valorNormalizado = NormalizarValor(match.Groups[1].Value);
            if (decimal.TryParse(valorNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor) && valor > 0)
                return valor;
        }

        return 0;
    }

    /// <summary>
    /// Extrai a data (e horário, quando houver) do texto da notificação.
    /// Quando não há data explícita, resolve o dia da semana dentro da semana corrente.
    /// Na ausência de qualquer referência, retorna a data de referência (agora).
    /// </summary>
    public static DateTime ExtrairDataHora(string texto, DateTime? referencia = null)
    {
        var agora = referencia ?? DateTime.Now;
        if (string.IsNullOrWhiteSpace(texto))
            return agora;

        var match = PadraoData.Match(texto);
        if (match.Success &&
            int.TryParse(match.Groups[1].Value, out var dia) &&
            int.TryParse(match.Groups[2].Value, out var mes) &&
            int.TryParse(match.Groups[3].Value, out var ano))
        {
            if (ano < 100)
                ano += 2000;

            var hora = 0;
            var minuto = 0;
            if (match.Groups[4].Success)
                int.TryParse(match.Groups[4].Value, out hora);
            if (match.Groups[5].Success)
                int.TryParse(match.Groups[5].Value, out minuto);

            if (dia is >= 1 and <= 31 && mes is >= 1 and <= 12)
                return new DateTime(ano, mes, dia, hora, minuto, 0);
        }

        // Sem data explícita: resolve pelo dia da semana (ex.: "Terça-feira * 09:17").
        var normalizado = RemoverAcentos(texto)?.ToLowerInvariant() ?? string.Empty;
        foreach (var (nome, diaSemana) in DiasSemana)
        {
            if (!normalizado.Contains(nome))
                continue;

            var hora = 0;
            var minuto = 0;
            var horaMatch = PadraoHora.Match(texto);
            if (horaMatch.Success)
            {
                int.TryParse(horaMatch.Groups[1].Value, out hora);
                int.TryParse(horaMatch.Groups[2].Value, out minuto);
            }

            var data = agora.Date;
            while (data.DayOfWeek != diaSemana)
                data = data.AddDays(-1);

            return data.AddHours(hora).AddMinutes(minuto);
        }

        return agora;
    }

    /// <summary>
    /// Extrai a descrição (estabelecimento) da notificação.
    /// Em compras usa o texto entre "em" e ", foi"; em pagamentos, entre "para" e "foi realizado".
    /// Sem correspondência, retorna o texto completo normalizado.
    /// </summary>
    public static string ExtrairDescricao(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return "***Não informado";

        var match = PadraoDescricaoCompra.Match(texto);
        if (match.Success)
        {
            var descricaoCompra = NormalizarEspacos(match.Groups[1].Value);
            if (!string.IsNullOrWhiteSpace(descricaoCompra))
                return LimitarDescricao(descricaoCompra);
        }

        match = PadraoDescricaoPagamento.Match(texto);
        if (match.Success)
        {
            var descricaoPagamento = NormalizarEspacos(match.Groups[1].Value);
            if (!string.IsNullOrWhiteSpace(descricaoPagamento))
                return LimitarDescricao(descricaoPagamento);
        }

        return LimitarDescricao(NormalizarEspacos(texto));
    }

    /// <summary>
    /// Infere se o movimento é crédito (entrada) ou débito (saída) a partir do texto.
    /// Na ausência de termos conhecidos assume débito.
    /// </summary>
    public static TipoMovimento ExtrairTipo(string texto)
    {
        var normalizado = RemoverAcentos(texto)?.ToLowerInvariant() ?? string.Empty;

        // "Compra no crédito" também é uma saída, por isso é tratada antes.
        if (normalizado.Contains("compra"))
            return TipoMovimento.Debito;

        return TermosCredito.Any(termo => normalizado.Contains(termo))
            ? TipoMovimento.Credito
            : TipoMovimento.Debito;
    }

    private static string LimitarDescricao(string descricao) =>
        descricao.Length > TamanhoMaximoDescricao
            ? descricao[..TamanhoMaximoDescricao]
            : descricao;

    private static string NormalizarEspacos(string texto) =>
        Regex.Replace(texto.Trim().Replace("\r", " ").Replace("\n", " "), @"\s{2,}", " ");

    private static string NormalizarValor(string valor)
    {
        valor = valor.Trim();

        // Formato brasileiro (1.234,56) usa vírgula como separador decimal.
        if (valor.Contains(','))
            return valor.Replace(".", string.Empty).Replace(",", ".");

        // Sem vírgula, um ponto seguido de exatamente 3 dígitos é separador de milhar (1.234).
        var partes = valor.Split('.');
        return partes.Length > 1 && partes[^1].Length == 3
            ? valor.Replace(".", string.Empty)
            : valor;
    }

    private static string RemoverAcentos(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return texto;

        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalizado.Length);

        foreach (var caractere in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
                builder.Append(caractere);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
