

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Areas.Admin.Controllers;

public class LogController(ILogRefit servico, IHttpContextAccessor httpContextAccessor)
    : Base.BaseAdminController<LogViewModel, Guid, ILogRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "DataHora desc";
}
