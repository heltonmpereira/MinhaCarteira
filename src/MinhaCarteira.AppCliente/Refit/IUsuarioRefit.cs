using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IUsuarioRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<UsuarioViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<UsuarioViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<UsuarioViewModel>> Alterar(UsuarioViewModel item);

    [Post("")]
    Task<RespostaServico<UsuarioViewModel>> Incluir(UsuarioViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}