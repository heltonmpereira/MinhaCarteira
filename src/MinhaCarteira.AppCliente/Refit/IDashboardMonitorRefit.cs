using MinhaCarteira.AppCliente.ViewModel;
using Refit;
using System.Threading.Tasks;
using System;
using MinhaCarteira.AppCliente.Models;

namespace MinhaCarteira.AppCliente.Refit;

public interface IDashboardMonitorRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<DashboardMonitorViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<DashboardMonitorViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<DashboardMonitorViewModel>> Alterar(DashboardMonitorViewModel item);

    [Post("")]
    Task<RespostaServico<DashboardMonitorViewModel>> Incluir(DashboardMonitorViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);
}
