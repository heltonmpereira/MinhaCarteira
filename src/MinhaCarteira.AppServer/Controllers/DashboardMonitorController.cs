using System;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;

namespace MinhaCarteira.AppServer.Controllers;

public class DashboardMonitorController(IDashboardMonitorServico servico, IHttpContextAccessor httpContextAcessor)
    : BaseApiController<DashboardMonitor, Guid, IDashboardMonitorServico, IDashboardMonitorRepositorio>(servico,
        httpContextAcessor);
