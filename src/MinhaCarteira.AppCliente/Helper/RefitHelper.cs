using Dhani.Utilitarios.Filtro;
using Dhani.Utilitarios.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinhaCarteira.AppCliente.Controllers.Interface;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Interface.Resposta;
using MinhaCarteira.AppCliente.ViewModel.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Helper;

public static class RefitHelper
{
    public static async Task<TRetorno> ExecutarServico<TRetorno, T, TPK>(
        this object servico,
        object[] parameters,
        [CallerMemberName] string nomeMetodo = null
    )
    {
        var metodo =
            servico.GetType().GetMethod(nomeMetodo)
            ?? throw new Exception(
                $"Não foi possível localizar o método '{nomeMetodo}' na API '{servico.GetType().Name}'."
            );
        var task = (Task)metodo.Invoke(servico, parameters);

        await task.ConfigureAwait(false);

        var resultProperty = task.GetType().GetProperty("Result");
        var valor = resultProperty.GetValue(task);

        var resultado = (TRetorno)valor;

        return resultado;
    }

    public static async Task<IActionResult> ChamarServicoPaginado<T, TPK, TSERVICO>(
        this IControllerPaginadoServico<TSERVICO> controller,
        ICriterio criterio,
        bool exibirRegistrosDeletados,
        string nomeMetodo = "Navegar",
        string nomeAction = "Index",
        bool serializarFiltro = true
    )
    {
        try
        {
            IRespostaPaginadaServico<T> itens = null;
            if (serializarFiltro)
            {
                var json = JsonConvert.SerializeObject(criterio.OrganizarIdFiltros());
                itens = await controller.Servico.ExecutarServico<
                    IRespostaPaginadaServico<T>,
                    T,
                    TPK
                >([json, exibirRegistrosDeletados], nomeMetodo);
            }
            else {
                itens = await controller.Servico.ExecutarServico<
                    IRespostaPaginadaServico<T>,
                    T,
                    TPK
                >([criterio.OrganizarIdFiltros(), exibirRegistrosDeletados], nomeMetodo);
            }


            criterio.Pagina = itens.NumeroPagina;
            //criterio.ItensPorPagina = itens.ItensPorPagina;

            var filtroViewModel = new CriterioViewModel(
                controller.ObterColunasFiltro(typeof(T)),
                criterio.GruposFiltro
            )
            {
                PaginaAtual = itens.NumeroPagina,
                ItensPorPagina = criterio.ItensPorPagina,
                Ordenacao = criterio.Ordenacao,
            };

            var itensPaginados = new ListaBaseViewModel<T>(itens)
            {
                Filtro = criterio,
                FiltroViewModel = filtroViewModel,
            };

            ActionResult viewRetorno = controller.View(nomeAction, itensPaginados);

            if (controller.TempData.TryGetValue("RetornoApi", out object value))
            {
                var retorno = value.ToString() ?? string.Empty;
                controller.TempData.Clear();
                controller.ViewBag.RetornoApi = JsonConvert.DeserializeObject<
                    RespostaServico<object>
                >(retorno);
            }

            return viewRetorno;
        }
        catch (ApiException ex)
        {
            var returnUrl = controller.HttpContext.Request.Path.Value;

            if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                return controller.RedirectToAction("Index", "Login", new
                {
                    area = "",
                    returnUrl
                });
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return controller.RedirectToAction(
                    "NaoAutorizado",
                    "Login",
                    new { area = "", returnUrl }
                );

            var retornoApi = await ex.GetContentAsAsync<RespostaServico<dynamic>>()
                .ConfigureAwait(false);
            retornoApi ??= new RespostaServico<dynamic>(ex, ex.Message)
            {
                BemSucedido = false,
                StatusCode = ex.StatusCode,
            };
            retornoApi.Mensagem ??= ex.Message;
            retornoApi.MensagemErro ??= ex.Content;
            if (!controller.TempData.TryGetValue("RetornoApi", out object value))
                controller.ViewBag.RetornoApi = retornoApi;
            else
            {
                var retorno = value.ToString() ?? string.Empty;
                controller.TempData.Clear();
                controller.ViewBag.RetornoApi = JsonConvert.DeserializeObject<
                    RespostaServico<object>
                >(retorno);
            }
        }
        catch (Exception e)
        {
            var retornoApi = new RespostaServico<Exception>(e, e.Message);
            controller.ViewBag.RetornoApi = retornoApi;
        }

        var view = controller.View(nomeAction, new ListaBaseViewModel<T>());
        return view;
    }

    public static async Task<IActionResult> ChamarServicoView<TRetorno, T, TPK, TSERVICO>(
        this IControllerServico<TSERVICO> controller,
        object model,
        bool redirecionarToIndex,
        string apiMetodo,
        string action,
        string actionSucesso,
        string nomePropriedade = null,
        string controllerName = null
    )
    {
        controllerName ??= controller.ControllerContext.RouteData.Values["controller"]?.ToString();

        try
        {
            var retorno = await controller.Servico.ExecutarServico<TRetorno, T, TPK>(
                parameters: model switch
                {
                    null => null,
                    Array => model as object[],
                    _ => [model],
                },
                nomeMetodo: apiMetodo
            );

            await controller.InicializarObjeto(retorno);
            if (!redirecionarToIndex)
            {
                var view = controller.View(actionSucesso, retorno.GetValue("Dados"));
                return await Task.FromResult(view);
            }

            var json = JObject.Parse(retorno.ToJson());
            json.Property("Dados")?.Remove();
            controller.TempData.Clear();
            controller.TempData["RetornoApi"] = json.ToString();

            actionSucesso =
                string.IsNullOrWhiteSpace(actionSucesso) || actionSucesso == apiMetodo
                    ? "index"
                    : actionSucesso;

            if (string.IsNullOrWhiteSpace(nomePropriedade))
            {
                var dados =
                    actionSucesso.ToLower() == "index" ? default : retorno.GetValue("Dados");

                var view = controller.RedirectToAction(actionSucesso, controllerName, dados);

                return view;
            }

            var paramDictionary = new RouteValueDictionary();
            var props = nomePropriedade.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var prop in props)
            {
                var valorParam = retorno.GetValue(prop);
                paramDictionary.Add(prop.Split(".").Last(), valorParam);
            }

            return controller.RedirectToAction(actionSucesso, controllerName, paramDictionary);
        }
        catch (ApiException ex)
        {
            var returnUrl = controller.HttpContext.Request.Path.Value;

            switch (ex.StatusCode)
            {
                case System.Net.HttpStatusCode.Forbidden:
                    return controller.RedirectToAction(
                        "Index",
                        "Login",
                        new { area = "", returnUrl }
                    );
                case System.Net.HttpStatusCode.Unauthorized:
                    return controller.RedirectToAction(
                        "NaoAutorizado",
                        "Login",
                        new { area = "", returnUrl }
                    );
            }

            var retornoApi = new RespostaServico<Exception>(ex, ex.Message)
            {
                StatusCode = ex.StatusCode,
            };
            try
            {
                var apiEx = await ex.GetContentAsAsync<RespostaServico<dynamic>>()
                    .ConfigureAwait(false);
                retornoApi.MensagemErro = apiEx?.Mensagem ?? ex.Message;
            }
            catch
            {
                //nada por aqui
            }

            if (!controller.TempData.ContainsKey("RetornoApi"))
                controller.ViewBag.RetornoApi = retornoApi;
            else
            {
                var retorno = controller.TempData["RetornoApi"].ToString() ?? string.Empty;
                controller.TempData.Clear();
                controller.ViewBag.RetornoApi = JsonConvert.DeserializeObject<
                    RespostaServico<object>
                >(retorno);
            }
        }
        catch (Exception e)
        {
            var retornoApi = new RespostaServico<Exception>(e, e.Message);
            controller.ViewBag.RetornoApi = retornoApi;
        }

        if (model != null && model.GetType() != typeof(T))
            model = default(T);

        return await Task.FromResult(controller.View(action, model));
    }

    public static async Task<IActionResult> ChamarServicoView<T, TPK, TSERVICO>(
        this IControllerServico<TSERVICO> controller,
        object model,
        [CallerMemberName] string apiMetodo = null,
        [CallerMemberName] string action = null
    )
    {
        return await controller.ChamarServicoView<RespostaServico<T>, T, TPK, TSERVICO>(
            model: model,
            redirecionarToIndex: false,
            apiMetodo: apiMetodo,
            action: action,
            actionSucesso: null,
            nomePropriedade: null
        );
    }

    public static async Task<IActionResult> ChamarServicoProsseguirIndex<
        TRetorno,
        T,
        TPK,
        TSERVICO
    >(
        this IControllerServico<TSERVICO> controller,
        object model,
        [CallerMemberName] string action = null,
        [CallerMemberName] string apiMetodo = null,
        string controllerName = null
    )
    {
        controllerName ??= controller.ControllerContext.RouteData.Values["controller"].ToString();

        if (typeof(TRetorno).Name.Contains("RespostaServico", StringComparison.OrdinalIgnoreCase))
            return await controller.ChamarServicoView<TRetorno, T, TPK, TSERVICO>(
                model: model,
                redirecionarToIndex: true,
                apiMetodo: apiMetodo,
                action: action,
                actionSucesso: null,
                nomePropriedade: null,
                controllerName
            );
        else
            return await controller.ChamarServicoView<RespostaServico<TRetorno>, T, TPK, TSERVICO>(
                model: model,
                redirecionarToIndex: true,
                apiMetodo: apiMetodo,
                action: action,
                actionSucesso: null,
                nomePropriedade: null,
                controllerName
            );
    }

    public static async Task<IActionResult> ChamarServicoProsseguirIndex<
        TRetorno,
        T,
        TPK,
        TSERVICO
    >(
        this IControllerServico<TSERVICO> controller,
        object model,
        string nomePropriedade,
        string actionSucesso,
        [CallerMemberName] string action = null,
        [CallerMemberName] string apiMetodo = null
    )
    {
        return await controller.ChamarServicoView<RespostaServico<TRetorno>, T, TPK, TSERVICO>(
            model: model,
            redirecionarToIndex: true,
            apiMetodo: apiMetodo,
            action: action,
            actionSucesso: actionSucesso,
            nomePropriedade: nomePropriedade
        );
    }

    public static async Task<IActionResult> ChamarServicoProsseguirIndex<T, TPK, TSERVICO>(
        this IControllerServico<TSERVICO> controller,
        object model,
        [CallerMemberName] string apiMetodo = null,
        [CallerMemberName] string action = null
    )
    {
        return await controller.ChamarServicoProsseguirIndex<RespostaServico<T>, T, TPK, TSERVICO>(
            model: model,
            apiMetodo: apiMetodo,
            action: action
        );
    }

    public static async Task<IActionResult> ChamarServicoProsseguirHomeIndex<T, TPK, TSERVICO>(
        this IControllerServico<TSERVICO> controller,
        object model,
        [CallerMemberName] string apiMetodo = null,
        [CallerMemberName] string action = null
    )
    {
        return await controller.ChamarServicoProsseguirIndex<RespostaServico<T>, T, TPK, TSERVICO>(
            model: model,
            apiMetodo: apiMetodo,
            action: action,
            controllerName: "home"
        );
    }
}
