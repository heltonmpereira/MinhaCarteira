using Refit;
using MinhaCarteira.AppCliente.ViewModel;
using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;

namespace MinhaCarteira.AppCliente.Refit;

public interface IRegraImportacaoRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<RegraImportacaoViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<RegraImportacaoViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<RegraImportacaoViewModel>> Alterar(RegraImportacaoViewModel item);

    [Post("")]
    Task<RespostaServico<RegraImportacaoViewModel>> Incluir(RegraImportacaoViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}
