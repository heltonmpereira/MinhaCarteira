using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;
using System;


namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class PessoaController(IPessoaRefit servico, IHttpContextAccessor httpContextAccessor)
    : BaseController<PessoaViewModel, Guid, IPessoaRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "Nome";
}
