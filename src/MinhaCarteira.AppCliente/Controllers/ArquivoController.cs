using System;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Controllers
{
    public class ArquivoController(IArquivoRefit servico, IHttpContextAccessor httpContextAccessor)
        : BaseController<IconeViewModel, Guid, IArquivoRefit>(servico, httpContextAccessor)
    {
    }
}
