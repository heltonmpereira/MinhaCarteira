using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IContaBancariaRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<ContaBancariaViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<ContaBancariaViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<ContaBancariaViewModel>> Alterar(ContaBancariaViewModel item);

    [Post("")]
    Task<RespostaServico<ContaBancariaViewModel>> Incluir(ContaBancariaViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);

    [Post("/incrementar-prioridade/{id}")]
    Task<RespostaServico<bool>> IncrementarPrioridade(Guid id);

    [Post("/decrementar-prioridade/{id}")]
    Task<RespostaServico<bool>> DecrementarPrioridade(Guid id);

    [Post("/reativar/{id}")]
    Task<RespostaServico<bool>> Reativar(Guid id);

    [Post("/atualizar-saldos")]
    Task<RespostaServico<bool>> AtualizarSaldos();

    [Post("/obter-contas-visiveis-na-tela-inicial")]
    Task<RespostaPaginadaServico<ContaBancariaViewModel>> ObterContasVisiveisNaTelaInicial();
}
