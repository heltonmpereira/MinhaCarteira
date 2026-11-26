using System;
using System.Net;
using Dhani.Utilitarios.Filtro;
using Dhani.Utilitarios.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Helper;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using Newtonsoft.Json;

namespace MinhaCarteira.AppServer.Controllers.Base;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PadraoApiController<TServico> : ControllerBase
{
    protected TServico Servico { get; }
    protected string IdUsuarioLogado { get; }
    protected bool UsuarioEhSuperAdmin { get; }
    protected string IdOrganizacaoUsuarioLogado { get; }
    protected virtual bool IgnorarValidacaoPosse { get; }
    protected virtual string IdValidacaoPosse => IdUsuarioLogado;
    protected virtual string PropriedadeValidacaoPosse => "ProprietarioId";
    protected virtual TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.FiltrarPorUsuarioLogado;
    public IHttpContextAccessor HttpContextAccessor { get; }

    private PadraoApiController(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
        IdUsuarioLogado = httpContextAccessor.HttpContext?.User?.FindFirst("UsuarioId")?.Value;
        IdOrganizacaoUsuarioLogado = httpContextAccessor.HttpContext?.User?.FindFirst("OrganizacaoId")?.Value;
        UsuarioEhSuperAdmin = httpContextAccessor.HttpContext?.User?.IsInRole("superadmin") ?? false;
        IgnorarValidacaoPosse = UsuarioEhSuperAdmin;
    }
    protected PadraoApiController(TServico servico, IHttpContextAccessor httpContextAccessor) : this(httpContextAccessor)
    {
        Servico = servico;
    }

    public PadraoApiController()
    {
    }

    protected IActionResult FalhaValidacaoUsuario() =>
        BadRequest(new RespostaServico<Usuario>(null, "Falha ao validar informacoes do usuario")
        {
            BemSucedido = false,
            StatusCode = HttpStatusCode.BadRequest,
            MensagemErro = "Falha ao validar informacoes do usuario"
        });

    protected virtual ICriterio ObterFiltroInicial(string criterioJson)
    {
        var criterio = string.IsNullOrWhiteSpace(criterioJson)
            ? new FiltroBase()
            : JsonConvert.DeserializeObject<FiltroBase>(criterioJson);

        criterio?.RenderizarValor();

        if (FiltroRegistros == TipoFiltroRegistroEnum.FiltrarPorUsuarioLogado)
            criterio.AdicionarFiltroProprietario(IdUsuarioLogado);
        else if (FiltroRegistros == TipoFiltroRegistroEnum.FiltrarPorUsuarioLogadoOuSemUsuarioEspecificado)
            criterio.AdicionarFiltroProprietarioOpcional(IdUsuarioLogado);
        else if (FiltroRegistros == TipoFiltroRegistroEnum.FiltrarPorOrganizacaoLogada && !UsuarioEhSuperAdmin)
            criterio.AdicionarFiltroProprietario(IdOrganizacaoUsuarioLogado, "OrganizacaoId");

        return criterio;
    }

    protected void DefinirUsuarioLogado<TEntidade>(TEntidade item)
    {
        const string nomePropriedade = "ProprietarioId";
        var prop = item.GetType().GetProperty(nomePropriedade);
        if (prop == null) return;

        if (Nullable.GetUnderlyingType(prop.PropertyType) == null)
            item.SetValue(nomePropriedade, IdUsuarioLogado);
    }

    protected IActionResult ConfrontarUsuarioLogadoEProprietario<TEntidade>(IActionResult respostaServico, string nomePropriedade = null)
    {
        if (IgnorarValidacaoPosse)
            return respostaServico;

        nomePropriedade ??= PropriedadeValidacaoPosse;

        if (string.IsNullOrWhiteSpace(IdValidacaoPosse))
            return FalhaValidacaoUsuario();

        if (respostaServico is OkObjectResult retornoOk)
        {
            if (retornoOk.Value is not IRespostaServico<TEntidade> resp || resp.Dados is null)
                return FalhaValidacaoUsuario();

            var propProprietarioId = resp.Dados.GetValue(nomePropriedade);

            if (propProprietarioId != null && (Guid)propProprietarioId != new Guid(IdValidacaoPosse))
                return FalhaValidacaoUsuario();
        }

        return respostaServico;
    }
}
