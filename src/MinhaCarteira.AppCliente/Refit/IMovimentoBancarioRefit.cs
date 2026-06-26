using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IMovimentoBancarioRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<MovimentoBancarioViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<MovimentoBancarioViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<MovimentoBancarioViewModel>> Alterar(MovimentoBancarioViewModel item);

    [Post("")]
    Task<RespostaServico<MovimentoBancarioViewModel>> Incluir(MovimentoBancarioViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);

    [Post("/transferir")]
    Task<RespostaServico<bool>> Transferir(MovimentoBancarioTransferenciaViewModel item);

    [Post("/conciliar-movimento")]
    Task<RespostaServico<int>> ConciliarMovimento(Guid idMovimento, string idParcelas);
}
