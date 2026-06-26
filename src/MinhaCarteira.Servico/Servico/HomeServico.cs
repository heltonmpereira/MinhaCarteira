using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Servico.Helper;

namespace MinhaCarteira.Servico.Servico;

public class HomeServico(
    IAgendamentoServico agendamentoServico,
    IContaBancariaServico contaBancariaServico,
    IDashboardMonitorServico dashboardMonitorServico,
    IMovimentoBancarioServico movimentoBancarioServico)
    : IHomeServico
{
    public async Task<IRespostaPaginadaServico<DashboardMonitor>> ObterDashboardMonitores(ICriterio criterio, string mesAno)
    {
        var monitores = await DashboardMonitorHelper.ObterMonitoresDashbaord(
            dashboardMonitorServico,
            movimentoBancarioServico,
            criterio,
            mesAno);

        var resposta = new RespostaPaginadaServico<DashboardMonitor>(monitores);
        return resposta;
    }

    public async Task<IRespostaPaginadaServico<DashboardResumo>> ObterDashboardResumo(ICriterio criterio, string mesAno)
    {
        var resumo = await DashboardMonitorHelper.ObterResumo(
            agendamentoServico,
            contaBancariaServico,
            movimentoBancarioServico,
            criterio,
            mesAno);

        var resposta = new RespostaPaginadaServico<DashboardResumo>([resumo]);
        return resposta;
    }
}