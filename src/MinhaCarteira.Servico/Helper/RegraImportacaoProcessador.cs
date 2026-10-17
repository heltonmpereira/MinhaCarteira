using System;
using System.Collections.Generic;
using System.Linq;
using MinhaCarteira.Definicao.Entidade;

namespace MinhaCarteira.Servico.Helper;

public static class RegraImportacaoProcessador
{
    public static List<MovimentoBancario> Processar(
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
