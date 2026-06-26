using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IInstituicaoFinanceiraRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<InstituicaoFinanceiraViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<InstituicaoFinanceiraViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<InstituicaoFinanceiraViewModel>> Alterar(InstituicaoFinanceiraViewModel item);

    [Post("")]
    Task<RespostaServico<InstituicaoFinanceiraViewModel>> Incluir(InstituicaoFinanceiraViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}
