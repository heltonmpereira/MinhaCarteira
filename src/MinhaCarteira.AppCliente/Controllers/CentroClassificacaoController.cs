using System;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class CentroClassificacaoController(ICentroClassificacaoRefit servico, IHttpContextAccessor httpContextAccessor)
    : BaseController<CentroClassificacaoViewModel, Guid, ICentroClassificacaoRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "Nome";
}
