using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;

namespace MinhaCarteira.Definicao.Interface.Repositorio;

public interface IAgendamentoRepositorio : IRepositorio<Agendamento, Guid>
{
    Task AtualizarAgendamento(Agendamento agend);
}
