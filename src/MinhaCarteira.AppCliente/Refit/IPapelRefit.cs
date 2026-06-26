using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IPapelRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<PapelViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<PapelViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<PapelViewModel>> Alterar(PapelViewModel item);

    [Post("")]
    Task<RespostaServico<PapelViewModel>> Incluir(PapelViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);

    [Put("/atualizar-usuarios")]
    Task<RespostaServico<PapelViewModel>> AtualizarUsuarios(Guid id, Guid[] idsUsuario);
}