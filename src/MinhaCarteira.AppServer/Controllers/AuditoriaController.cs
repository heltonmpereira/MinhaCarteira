using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;

namespace MinhaCarteira.AppServer.Controllers;

[Authorize(Roles = "superadmin,admin")]
[ApiController]
[Route("api/[controller]")]
public class AuditoriaController(IAuditoriaServico servico, IHttpContextAccessor httpContextAccessor) 
    : BaseLeituraApiController<Auditoria, Guid, IAuditoriaServico>(servico, httpContextAccessor)
{
    protected override bool IgnorarValidacaoPosse => UsuarioEhSuperAdmin;
    protected override string PropriedadeValidacaoPosse => "OrganizacaoId";
    protected override string IdValidacaoPosse => IdOrganizacaoUsuarioLogado;
    protected override TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.FiltrarPorOrganizacaoLogada;
}
