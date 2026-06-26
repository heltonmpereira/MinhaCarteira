using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Helper;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Definicao.Modelo.Extrato;
using MinhaCarteira.Servico.Helper;
using MinhaCarteira.Servico.Servico.Base;

namespace MinhaCarteira.Servico.Servico;

public class ImportarArquivoServico(
    IImportarArquivoRepositorio repositorio,
    IMovimentoBancarioServico movimentoBancarioServico,
    IRegraImportacaoServico regraImportacaoServico,
    IContaBancariaServico contaBancariaServico)
    : BaseServico<ImportarArquivo, Guid, IImportarArquivoRepositorio>(repositorio), IImportarArquivoServico
{
    public async Task<IRespostaPaginadaServico<MovimentoBancario>> ProcessarArquivo(FileUploadModel model)
    {
        if (model.ContaBancariaId != Guid.Empty)
        {
            var contaDb = await contaBancariaServico.ObterPorId(model.ContaBancariaId);
            model.ContaBancaria = contaDb.Dados;
        }

        var criterio = new FiltroBase { AdicionarIncludes = true, ItensPorPagina = 1 };
        criterio.AdicionarFiltroProprietario(model.ProprietarioId.ToString());

        var regras = await regraImportacaoServico.Navegar(criterio);

        var importacao = new ImportacaoExtrato(model);
        var registros = await importacao.ExtrairRegistros(
            proprietarioId: model.ProprietarioId,
            regras: regras.Dados,
            pessoaPadrao: null,
            categoriaPadrao: null,
            centroClassificacaoPadrao: null);

        var retorno = new RespostaPaginadaServico<MovimentoBancario>(registros)
        {
            BemSucedido = registros.Count > 0,
            Mensagem = "Registros processados com sucesso."
        };

        return retorno;
    }

    public async Task<IRespostaServico<bool>> ImportarMovimentos(List<MovimentoBancario> itens)
    {
        var movimentosValidos = itens
            .Where(w => w.PessoaId != Guid.Empty &&
                        w.CategoriaId != Guid.Empty &&
                        w.CentroClassificacaoId != Guid.Empty)
            .ToList();

        if (!movimentosValidos.Any())
            return new RespostaServico<bool>(false);

        var import = new ImportarArquivo()
        {
            ContaBancariaId = itens.FirstOrDefault()!.ContaBancariaId,
            ProprietarioId = itens.FirstOrDefault()!.ProprietarioId
        };
        var importDb = await Incluir(import);
        movimentosValidos.ForEach(f => f.ImportarArquivoId = importDb.Dados.Id);
        var itensDb = await movimentoBancarioServico.IncluirRange(movimentosValidos);

        var retorno = new RespostaServico<bool>(itensDb.Dados > 0);
        return retorno;

    }
}
