using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface ICentroClassificacaoRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<CentroClassificacaoViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<CentroClassificacaoViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<CentroClassificacaoViewModel>> Alterar(CentroClassificacaoViewModel item);

    [Post("")]
    Task<RespostaServico<CentroClassificacaoViewModel>> Incluir(CentroClassificacaoViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}
