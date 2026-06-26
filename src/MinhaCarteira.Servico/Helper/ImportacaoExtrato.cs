using ChoETL;
using Dhani.Utilitarios.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Definicao.Modelo.Extrato;
using Scriban;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinhaCarteira.Servico.Helper;

public class ImportacaoExtrato(FileUploadModel model)
{
    private static IEnumerable<string> CarregarConteudo(byte[] byteArray)
    {
        var texto = Encoding.UTF8.GetString(byteArray);
        var linhas = texto.Split(
            ["\r\n", "\r", "\n"],
            StringSplitOptions.RemoveEmptyEntries);

        return linhas;
    }

    public async Task<List<MovimentoBancario>> ExtrairRegistros(
        Guid proprietarioId,
        IList<RegraImportacao> regras,
        Pessoa pessoaPadrao,
        Categoria categoriaPadrao,
        CentroClassificacao centroClassificacaoPadrao)
    {
        return Path.GetExtension(model.NomeArquivo)?.ToLower() switch
        {
            ".ofx" => await ExtrairRegistrosOfx(proprietarioId, regras, pessoaPadrao, categoriaPadrao, centroClassificacaoPadrao),
            _ => await ExtrairRegistrosCsv(proprietarioId, regras, pessoaPadrao, categoriaPadrao, centroClassificacaoPadrao),
        };
    }

    static DateTime ParseCustomDateTime(string input)
    {
        // Regex para capturar a data e o fuso horário
        var regex = new Regex(@"(\d{14})\[(\d{1,2}|[-+]\d{1,2}):\w{3}\]|(\d{14})|(\d{08})");
        var match = regex.Match(input);

        if (!match.Success)
        {
            throw new FormatException("Formato de entrada não reconhecido.");
        }

        if (match.Groups[1].Value != null)
        {
            // Extrair partes
            string datePart = match.Groups[1].Value; // "20241028000000"
            int offsetHours = int.Parse(match.Groups[2].Value); // "-3"

            // Converter a string para DateTime
            DateTime dateTime = DateTime.ParseExact(datePart, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            // Aplicar o fuso horário
            DateTime dateTimeWithOffset = dateTime.AddHours(offsetHours);

            return dateTimeWithOffset;
        }

        if (match.Groups[3].Value != null)
        {
            string datePart = match.Groups[3].Value; // "20241028000000"
            DateTime dateTime = DateTime.ParseExact(datePart, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return dateTime;
        }

        if (match.Groups[4].Value != null)
        {
            string datePart = match.Groups[4].Value; // "20241028"
            DateTime dateTime = DateTime.ParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture);
            return dateTime;
        }

        throw new FormatException($"Não foi possível converter o valor {input} em uma data e hora válida.");
    }

    private async Task<List<MovimentoBancario>> ExtrairRegistrosOfx(
        Guid proprietarioId,
        IList<RegraImportacao> regras,
        Pessoa pessoaPadrao,
        Categoria categoriaPadrao,
        CentroClassificacao centroClassificacaoPadrao)
    {
        var linhas = CarregarConteudo(model.Conteudo);
        var doc = ImportOfx.ToXElement(linhas);
        try
        {
            var itens = doc
                .Descendants("STMTTRN")
                .Select(s => new MovimentoBancario
                {
                    ProprietarioId = proprietarioId,
                    ContaBancariaId = model.ContaBancariaId,
                    Descricao = s.Element("MEMO")?.Value,
                    TipoMovimento = string.IsNullOrWhiteSpace(s.Element("TRNTYPE")?.Value)
                        ? TipoMovimento.Debito
                        : s.Element("TRNTYPE")!.Value.Equals("DEBIT", StringComparison.OrdinalIgnoreCase)
                            ? TipoMovimento.Debito
                            : s.Element("TRNTYPE")!.Value.Equals("OTHER", StringComparison.OrdinalIgnoreCase)
                            && s.Element("TRNAMT")!.Value.Contains('-', StringComparison.OrdinalIgnoreCase)
                                ? TipoMovimento.Debito
                                : TipoMovimento.Credito,
                    Valor = string.IsNullOrWhiteSpace(s.Element("TRNAMT")?.Value)
                        ? 0
                        : Math.Abs(Convert.ToDecimal(s.Element("TRNAMT")?.Value.Replace("-", "")) / 100),
                    DataMovimento = ParseCustomDateTime(s.Element("DTPOSTED")?.Value),
                    Competencia = string.IsNullOrWhiteSpace(s.Element("DTPOSTED")?.Value)
                        ? DateTime.Now.ToString("yyyyMM")
                        : ParseCustomDateTime(s.Element("DTPOSTED")?.Value).ToString("yyyyMM")
                })
                .ToList();

            itens = ProcessarRegras(itens, regras, pessoaPadrao, categoriaPadrao, centroClassificacaoPadrao);
            return await Task.FromResult(itens);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<List<MovimentoBancario>> ExtrairRegistrosCsv(
        Guid proprietarioId,
        IList<RegraImportacao> regras,
        Pessoa pessoaPadrao,
        Categoria categoriaPadrao,
        CentroClassificacao centroClassificacaoPadrao)
    {
        var linhas = CarregarConteudo(model.Conteudo);

        var sb = new StringBuilder();
        foreach (var linha in linhas)
            sb.AppendLine(linha);

        var delimitador = model.ContaBancaria.InstituicaoFinanceira.DelimitadorCsv ?? ";";
        var ds = new DataSet { EnforceConstraints = false };
        using var reader = !model.ContaBancaria.InstituicaoFinanceira.NomeCamposPrimeiraLinha
                   ? ChoCSVReader.LoadText(sb.ToString()).WithDelimiter(delimitador)
                   : ChoCSVReader.LoadText(sb.ToString()).WithDelimiter(delimitador).WithFirstLineHeader();
        ds.Tables.Add("Extrato");
        ds.Tables[0].Load(reader.AsDataReader());

        var itens = new List<MovimentoBancario>();
        var mapeamento = model
            .ContaBancaria?
            .InstituicaoFinanceira?
            .MapeamentoCamposCsv;

        if (mapeamento != null)
        {
            var campos = mapeamento
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split("=>", ChoStringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(key => key[0], val => val[1]);


            foreach (DataRow dataRow in ds.Tables[0].Rows)
            {
                var row = new Dictionary<string, object>();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    var nomeColuna = col.ColumnName
                        .Replace(" ", "_")
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace("$", "")
                        .Replace("/", "");
                    row.Add(nomeColuna, dataRow[col]);
                }

                var mov = new MovimentoBancario
                {
                    Competencia = string.Empty,
                    ProprietarioId = proprietarioId,
                    ContaBancariaId = model.ContaBancariaId,
                };

                foreach (var campo in campos)
                {
                    var template = Template.Parse(campo.Value);
                    try
                    {
                        var valor = await template.RenderAsync(new { model = row });

                        mov.SetValue(campo.Key, valor);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                itens.Add(mov);
            }
        }

        itens = ProcessarRegras(itens, regras, pessoaPadrao, categoriaPadrao, centroClassificacaoPadrao);
        return await Task.FromResult(itens);
    }

    private static List<MovimentoBancario> ProcessarRegras(
        List<MovimentoBancario> movimentos,
        IList<RegraImportacao> regras,
        Pessoa pessoaPadrao,
        Categoria categoriaPadrao,
        CentroClassificacao centroClassificacaoPadrao)
    {
        foreach (var regra in regras)
        {
            List<MovimentoBancario> itensAProcessar = null;
            if (regra.ContaBancariaId != null)
                itensAProcessar = movimentos
                    .Where(w => w.ContaBancariaId == regra.ContaBancariaId)
                    .ToList();
            else itensAProcessar = movimentos;

            var palavrasChaves = regra.PalavrasChave
                .Replace("\\r", "")
                .Replace("\\n", "")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.ToLower())
                .ToArray();

            foreach (var palavraChave in palavrasChaves)
            {
                List<MovimentoBancario> itens = null;
                if (palavraChave.StartsWith('*') && palavraChave.EndsWith("*"))
                    itens = itensAProcessar
                        .Where(w => w.Descricao.ToLower().Trim().Equals(palavraChave.Replace("*", "").Trim()))
                        .ToList();
                else
                    itens = itensAProcessar
                        .Where(w => w.Descricao.ToLower().Contains(palavraChave))
                        .ToList();

                if (regra.ValorMinimo != null && regra.ValorMinimo > 0)
                    itens = itens
                        .Where(w => w.Valor >= regra.ValorMinimo)
                        .ToList();

                if (regra.ValorMaximo != null && regra.ValorMaximo > 0)
                    itens = itens
                        .Where(w => w.Valor <= regra.ValorMaximo)
                        .ToList();

                foreach (var item in itens)
                {
                    item.Pessoa = regra.Pessoa;
                    item.PessoaId = regra.PessoaId;

                    item.Categoria = regra.Categoria;
                    item.CategoriaId = regra.CategoriaId;

                    item.CentroClassificacao = regra.CentroClassificacao;
                    item.CentroClassificacaoId = regra.CentroClassificacaoId;
                }
            }
        }

        if (pessoaPadrao == null || categoriaPadrao == null || centroClassificacaoPadrao == null)
            return movimentos;

        foreach (var item in movimentos.Where(w => w.Pessoa == null))
        {
            item.Pessoa = pessoaPadrao;
            item.PessoaId = pessoaPadrao.Id;

            item.Categoria = categoriaPadrao;
            item.CategoriaId = categoriaPadrao.Id;

            item.CentroClassificacao = centroClassificacaoPadrao;
            item.CentroClassificacaoId = centroClassificacaoPadrao.Id;
        }

        return movimentos;
    }
}
