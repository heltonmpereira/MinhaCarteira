
using System;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Areas.Admin.Controllers;

public class AuditoriaController(IAuditoriaRefit servico, IHttpContextAccessor httpContextAccessor)
    : Base.BaseAdminController<AuditoriaViewModel, Guid, IAuditoriaRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "DataHora desc";
}
