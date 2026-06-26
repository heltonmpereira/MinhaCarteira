using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;
using System;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Refit;

public interface IArquivoRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<IconeViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<IconeViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<IconeViewModel>> Alterar(IconeViewModel item);

    [Post("")]
    Task<RespostaServico<IconeViewModel>> Incluir(IconeViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}
