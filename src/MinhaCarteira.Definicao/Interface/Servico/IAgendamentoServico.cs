using System;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IAgendamentoServico : IServico<Agendamento, Guid, IAgendamentoRepositorio>
{
    Task<IRespostaPaginadaServico<AgendamentoParcela>> ContasAVencer(ICriterio filtro);
    Task<IRespostaServico<AgendamentoParcela>> ObterParcelaPorId(Guid id);
    Task<IRespostaServico<AgendamentoParcela>> AlterarParcela(AgendamentoParcela parcela);
    Task<IRespostaServico<int>> DeletarParcela(Guid idParcela);
    Task<IRespostaServico<AgendamentoParcela>> PagarParcela(AgendamentoParcela parcela);
    Task<IRespostaServico<int>> RestaurarCondicaoParcela(Guid idParcela);
    Task<IRespostaServico<int>> ConciliarParcela(Guid proprietarioId, Guid idParcela, string idMovimentos);
    Task<IRespostaServico<bool>> ConciliarParcelasAPartirDashboardMonitor(string mesANo);
    Task<IRespostaServico<Agendamento>> EstenderParcelasPor5Anos(Guid idAgendamento);
}
