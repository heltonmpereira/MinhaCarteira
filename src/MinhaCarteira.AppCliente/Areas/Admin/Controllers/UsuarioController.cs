using System;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Areas.Admin.Controllers;

public class UsuarioController(IUsuarioRefit servico, IHttpContextAccessor httpContextAccessor)
    : Base.BaseAdminController<UsuarioViewModel, Guid, IUsuarioRefit>(servico, httpContextAccessor);