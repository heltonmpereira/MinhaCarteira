using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dhani.Utilitarios.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Controllers.Interface;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Helper;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Interface.Resposta;
using MinhaCarteira.AppCliente.Models.Usuario;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;
using MinhaCarteira.AppCliente.ViewModel.Login;
using Refit;

namespace MinhaCarteira.AppCliente.Controllers;

//[AllowAnonymous]
[Microsoft.AspNetCore.Authorization.Authorize]
public class LoginController(ILoginRefit servico) : PadraoController, IControllerServico<ILoginRefit>
{
    public ILoginRefit Servico { get; } = servico;

    public virtual Task InicializarObjeto(object instancia)
    {
        //if (instancia == null)
        return Task.CompletedTask;
    }

    [AllowAnonymous]
    public IActionResult NaoAutorizado(string returnUrl)
    {
        ModelState.AddModelError(string.Empty, "Acesso não autorizado.");
        return View(nameof(Index));
    }
    [AllowAnonymous]
    public IActionResult Index(string returnUrl)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel conta)
    {
        if (!ModelState.IsValid)
            return View(conta);

        try
        {
            var view = await this.ChamarServicoProsseguirIndex<UsuarioToken, UsuarioViewModel, Guid, ILoginRefit>(
                model: conta,
                nomePropriedade: "Dados",
                actionSucesso: "Logar",
                action: "Index",
                apiMetodo: "Logar");

            if (view is ViewResult result)
            {
                var retornoApi = result.ViewData.FirstOrDefault().Value as IRespostaServico<Exception>;
                ModelState.AddModelError(string.Empty, retornoApi.MensagemErro);
                result.ViewData.Clear();

                return view;
            }

            if (view is RedirectToActionResult redirect)
            {
                var value = redirect.RouteValues?.Values?.FirstOrDefault();
                var usuarioToken = value as UsuarioToken;
                usuarioToken.ManterConectado = conta.ManterConectado;
                await ArmazenarLoginEmCookie(usuarioToken);
                return Url.IsLocalUrl(conta.ReturnUrl)
                    ? Redirect(conta.ReturnUrl)
                    : RedirectToAction("Index", "Home");
            }
        }
        catch (ApiException ex)
        {
            var retornoApi = await ex
                .GetContentAsAsync<RespostaServico<ApiException>>()
                .ConfigureAwait(false);

            ModelState.AddModelError(string.Empty, retornoApi.Mensagem);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
        }

        return View(conta);
    }
    private async Task ArmazenarLoginEmCookie(UsuarioToken user, string usuarioLogado = "", string usuarioLogadoId = "")
    {
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new("UsuarioId", user.Id.ToString()),
            new(ClaimTypes.Name, user.Nome),
            new(ClaimTypes.Surname, user.Sobrenome),
            new(ClaimTypes.NameIdentifier, user.Username),
            new("FullName", user.NomeCompleto ?? user.Username),
            new("AcessadoVia", usuarioLogado),
            new("AcessadoViaId", usuarioLogadoId)
        }, Constante.NOME_COOKIE_ASPNET);

        claimsIdentity.AddClaims(
            user.Roles
                .Select(s => new Claim(ClaimTypes.Role, s.Trim().ToLower()))
                .ToArray());

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var props = new AuthenticationProperties()
        {
            AllowRefresh = true
        };
        if (user.ManterConectado)
        {
            props.IsPersistent = true;
            props.ExpiresUtc = user.DataExpericaoToken.ToUniversalTime();
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            props);

        Response.CriarCookie(Constante.NOME_COOKIE_BEARER, user.TokenAcesso);
    }

    [AllowAnonymous]
    public IActionResult Voltar(string returnUrl)
    {
        return Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> AcessarComo(Guid id, bool sairAcessoComo = false)
    {
        var idUsuarioLogado = User.FindFirstValue("UsuarioId");
        if (idUsuarioLogado == null || idUsuarioLogado == id.ToString())
        {
            var msg = "Escolha outro usuário para simular o acesso";
            var excecao = new RespostaServico<Exception>(new Exception(msg));
            TempData["RetornoApi"] = excecao.ToJson();
            return RedirecionarPaginaInicialArea("Admin", "Usuario");
        }

        var view = await this.ChamarServicoView<UsuarioToken, Guid, ILoginRefit>(id);

        if (view is ViewResult result)
        {
            if (result.Model != null && result.Model is UsuarioToken token)
            {
                var usuarioLogado = User.FindFirstValue("FullName");
                await Logout();
                if (sairAcessoComo)
                    await ArmazenarLoginEmCookie(token);
                else await ArmazenarLoginEmCookie(token, usuarioLogado, idUsuarioLogado);

                return RedirecionarPaginaInicialSite();
            }

            var retornoApi = result.ViewData.FirstOrDefault().Value;
            TempData["RetornoApi"] = retornoApi.ToJson();
        }
        return RedirecionarPaginaInicialArea("Admin", "Usuario");
    }

    [AllowAnonymous]
    public IActionResult Registrar()
    {
        return View();
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Registrar(RegistrarOrganizacaoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        return await this.ChamarServicoProsseguirIndex<UsuarioViewModel, Guid, ILoginRefit>(model);
    }

    public async Task<IActionResult> Sair()
    {
        await Logout().ConfigureAwait(false);

        return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> SairAcessoComo()
    {
        var idUsuarioLogado = User.FindFirstValue("AcessadoViaId");
        return await AcessarComo(new Guid(idUsuarioLogado), true);
    }
    private async Task Logout()
    {
        Response.Cookies.Delete(Constante.NOME_COOKIE_BEARER);

        await HttpContext
            .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
            .ConfigureAwait(false);
    }

    [AllowAnonymous]
    public IActionResult RedefinirSenha()
    {
        return View();
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> RedefinirSenha(SolicitarRedefinicaoSenhaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var view = await this.ChamarServicoProsseguirIndex<
            RedefinirSenhaViewModel,
            UsuarioViewModel,
            Guid,
            ILoginRefit>(
                model: model,
                nomePropriedade: "Dados.CodigoRedefinicaoSenha;Dados.id",
                actionSucesso: nameof(SolicitarAlteracaoSenhaComCodigoRedefinicao));

        if (view is ViewResult result)
        {
            var retornoApi = result.ViewData.FirstOrDefault().Value as IRespostaServico<Exception>;
            ModelState.AddModelError(string.Empty, retornoApi.MensagemErro);
            result.ViewData.Clear();

            return view;
        }

        return view;
    }

    [AllowAnonymous]
    [Route("[controller]/[action]/{id}/{codigoRedefinicaoSenha}")]
    public IActionResult SolicitarAlteracaoSenhaComCodigoRedefinicao(string id, string codigoRedefinicaoSenha)
    {
        var model = new RedefinirSenhaViewModel()
        {
            Id = new Guid(id),
            CodigoRedefinicaoSenha = codigoRedefinicaoSenha
        };
        return View(nameof(AlterarSenhaComCodigoRedefinicao), model);
    }
    [AllowAnonymous]
    [HttpPost]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> AlterarSenhaComCodigoRedefinicao(RedefinirSenhaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        return await this.ChamarServicoProsseguirIndex<bool, UsuarioViewModel, Guid, ILoginRefit>(model);
    }

    [ConfrontarUsuarioLogado]
    public async Task<IActionResult> MeuPerfil(Guid id)
    {
        return await this.ChamarServicoView<UsuarioViewModel, Guid, ILoginRefit>(id, "ObterPorId");
    }
    [HttpPost]
    [ConfrontarUsuarioLogado]
    public async Task<IActionResult> MeuPerfil(UsuarioViewModel model)
    {
        if (model.Id.ToString() != User.FindFirstValue("UsuarioId"))
        {
            ModelState.AddModelError(string.Empty, "Dados inválidos.");
            return View(model);
        }

        return await this.ChamarServicoProsseguirHomeIndex<UsuarioViewModel, Guid, ILoginRefit>(model, "Alterar");
    }

    [AllowAnonymous]
    public IActionResult AlterarSenha(Guid id)
    {
        var model = new AlterarSenhaViewModel { Id = id };

        return View(model);
    }
    [AllowAnonymous]
    [HttpPost]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        return await this.ChamarServicoProsseguirIndex<bool, UsuarioViewModel, Guid, ILoginRefit>(model);
    }
}