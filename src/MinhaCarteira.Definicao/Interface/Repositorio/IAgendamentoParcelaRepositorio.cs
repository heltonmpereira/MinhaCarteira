using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;

namespace MinhaCarteira.Definicao.Interface.Repositorio;

public interface IAgendamentoParcelaRepositorio : Base.IRepositorio<AgendamentoParcela, Guid>
{
    Task<bool> ConciliarParcelasAPartirDashboardMonitor();
    Task<bool> BaixarParcela(AgendamentoParcela parcela);
}
