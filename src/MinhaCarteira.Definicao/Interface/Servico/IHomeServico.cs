using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IHomeServico
{
    Task<IRespostaPaginadaServico<DashboardMonitor>> ObterDashboardMonitores(ICriterio criterio, string mesAno);
    Task<IRespostaPaginadaServico<DashboardResumo>> ObterDashboardResumo(ICriterio criterio, string mesAno);
}
