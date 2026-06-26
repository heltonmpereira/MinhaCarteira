using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Refit;

public interface IHomeRefit
{
    [Post("/obter-dashboard-monitores")]
    Task<RespostaPaginadaServico<DashboardMonitorViewModel>> ObterDashboardMonitores(string mesAno);

    [Post("/obter-dashboard-resumo")]
    Task<RespostaPaginadaServico<DashboardResumoViewModel>> ObterDashboardResumo(string mesAno);
}
