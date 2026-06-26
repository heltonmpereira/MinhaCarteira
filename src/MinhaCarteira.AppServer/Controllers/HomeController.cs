using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.AppServer.Controllers;

public class HomeController(IHomeServico servico, IHttpContextAccessor httpContextAccessor)
    : PadraoApiController<IHomeServico>(servico, httpContextAccessor)
{
    [HttpPost("obter-dashboard-monitores")]
    public async Task<IActionResult> ObterDashboardMonitores(string mesAno) =>
        await Servico.RespostaPaginadaServicoAsync<DashboardMonitor>(
             parameters: [ObterFiltroInicial(null), mesAno]);

    [HttpPost("obter-dashboard-resumo")]
    public async Task<IActionResult> ObterDashboardResumo(string mesAno) =>
        await Servico.RespostaPaginadaServicoAsync<DashboardResumo>(
             parameters: [ObterFiltroInicial(null), mesAno]);
}
