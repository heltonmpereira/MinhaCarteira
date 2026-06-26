using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IPessoaRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<PessoaViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<PessoaViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<PessoaViewModel>> Alterar(PessoaViewModel item);

    [Post("")]
    Task<RespostaServico<PessoaViewModel>> Incluir(PessoaViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}
