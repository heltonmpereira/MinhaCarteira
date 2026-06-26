using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;
using System;


namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class ExtraRegraImportacaoController(IRegraImportacaoRefit servico, IHttpContextAccessor httpContextAccessor)
    : BaseController<RegraImportacaoViewModel, Guid, IRegraImportacaoRefit>(servico, httpContextAccessor);
