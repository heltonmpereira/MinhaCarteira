using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using System.Threading.Tasks;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IHomeServico
{
    Task<IRespostaPaginadaServico<DashboardMonitor>> ObterDashboardMonitores(ICriterio criterio, string mesAno);
    Task<IRespostaPaginadaServico<DashboardResumo>> ObterDashboardResumo(ICriterio criterio, string mesAno);
}
