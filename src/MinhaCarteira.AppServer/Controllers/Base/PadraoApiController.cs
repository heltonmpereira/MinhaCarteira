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
public abstract class PadraoApiController<TServico> : ControllerBase
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

    protected void AtribuirItemAoUsuarioLogado<TEntidade>(TEntidade item)
    {
        const string nomePropriedade = "ProprietarioId";
        var prop = item.GetType().GetProperty(nomePropriedade);
        if (prop == null) return;

        if (Nullable.GetUnderlyingType(prop.PropertyType) == null)
            item.SetValue(nomePropriedade, IdUsuarioLogado);
    }

    protected IActionResult ConfrontarUsuarioLogadoEProprietario<TEntidade>(
        IActionResult respostaServico, string nomePropriedade = null) =>
        ConfrontarUsuarioLogadoEProprietario(respostaServico, nomePropriedade, typeof(TEntidade));

    protected IActionResult ConfrontarUsuarioLogadoEProprietario(
        IActionResult respostaServico, string nomePropriedade = null, Type tipo = null)
    {
        if (IgnorarValidacaoPosse)
            return respostaServico;

        nomePropriedade ??= PropriedadeValidacaoPosse;

        if (string.IsNullOrWhiteSpace(IdValidacaoPosse))
            return new FalhaValidacaoUsuarioResult();

        if (respostaServico is OkObjectResult retornoOk)
        {
            var resposta = retornoOk.Value;

            if (resposta is null
                || (tipo != null && !typeof(IRespostaServico<>).MakeGenericType(tipo).IsInstanceOfType(resposta)))
                return new FalhaValidacaoUsuarioResult();

            var dados = resposta.GetValue("Dados");
            if (dados is null)
                return new FalhaValidacaoUsuarioResult();

            var propProprietarioId = dados.GetValue(nomePropriedade);

            if (propProprietarioId != null && (Guid)propProprietarioId != new Guid(IdValidacaoPosse))
                return new FalhaValidacaoUsuarioResult();
        }

        return respostaServico;
    }
}
