using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using System;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IDashboardMonitorServico : IServico<DashboardMonitor, Guid, IDashboardMonitorRepositorio>
{
}
