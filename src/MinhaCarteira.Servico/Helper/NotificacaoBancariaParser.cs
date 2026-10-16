using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.Servico.Helper;

public static class NotificacaoBancariaParser
{
    private const int TamanhoMaximoDescricao = 200;

    // Padrões de valor monetário, do mais específico para o mais genérico.
    private static readonly Regex[] PadroesValor =
    [
        new(@"R\$\s*(\d{1,3}(?:\.\d{3})*,\d{2})", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"(\d{1,3}(?:\.\d{3})+,\d{2})", RegexOptions.Compiled),
        new(@"(\d+,\d{2})", RegexOptions.Compiled),
        new(@"R\$\s*(\d{1,3}(?:\.\d{3})+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"R\$\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"(\d+\.\d{2})", RegexOptions.Compiled)
    ];

    private static readonly string[] TermosCredito =
    [
        "receb", "credit", "credito", "entrada", "deposit", "estorno",
        "reembolso", "resgate", "rendimento", "salario"
    ];

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

    /// <summary>
    /// Retorna o texto da notificação como descrição do movimento, respeitando o limite da coluna.
    /// </summary>
    public static string ExtrairDescricao(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        var descricao = texto.Trim().Replace("\r", " ").Replace("\n", " ");
        return descricao.Length > TamanhoMaximoDescricao
            ? descricao[..TamanhoMaximoDescricao]
            : descricao;
    }

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
