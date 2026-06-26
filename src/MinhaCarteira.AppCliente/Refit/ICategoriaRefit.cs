using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface ICategoriaRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<CategoriaViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<CategoriaViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<CategoriaViewModel>> Alterar(CategoriaViewModel item);

    [Post("")]
    Task<RespostaServico<CategoriaViewModel>> Incluir(CategoriaViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}
