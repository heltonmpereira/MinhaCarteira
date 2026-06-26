using System;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using MinhaCarteira.AppCliente.ViewModel.Base;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IAgendamentoRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<AgendamentoViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<AgendamentoViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<AgendamentoViewModel>> Alterar(AgendamentoViewModel item);

    [Post("")]
    Task<RespostaServico<AgendamentoViewModel>> Incluir(AgendamentoViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);

    /* PARCELAS */
    [Post("/contas-a-vencer")]
    Task<RespostaPaginadaServico<AgendamentoParcelaViewModel>> ContasAVencer([Body] FiltroBase criterioJson, bool exibirRegistrosDeletados);

    [Get("/obter-parcela/{id}")]
    Task<RespostaServico<AgendamentoParcelaViewModel>> ObterParcelaPorId(Guid id);

    [Post("/alterar-parcela")]
    Task<RespostaServico<AgendamentoParcelaViewModel>> AlterarParcela(AgendamentoParcelaViewModel item);

    [Post("/deletar-parcela/{id}")]
    Task<RespostaServico<int>> DeletarParcela(Guid id);

    [Post("/pagar-parcela")]
    Task<RespostaServico<AgendamentoParcelaViewModel>> PagarParcela(AgendamentoParcelaViewModel item);

    [Post("/restaurar-condicao-parcela")]
    Task<RespostaServico<int>> RestaurarCondicaoParcela(Guid idParcela);

    [Post("/conciliar-parcela")]
    Task<RespostaServico<int>> ConciliarParcela(Guid idParcela, string idMovimentos);

    [Post("/conciliar-parcelas-a-partir-dashboard-monitor")]
    Task<RespostaServico<bool>> ConciliarParcelasAPartirDashboardMonitor(string mesANo);

    [Post("/estender-parcelas-5-anos/{id}")]
    Task<RespostaServico<AgendamentoViewModel>> EstenderParcelasPor5Anos(Guid id);
}
