using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace MinhaCarteira.AppCliente.Filter;

public class ConfrontarUsuarioLogadoAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (!ConfrontarUsuario(context))
            context.Result = new RedirectToRouteResult(
            new RouteValueDictionary
            {
            { "controller", "Login" },
            { "action", "Index" },
            { "returnUrl", context.HttpContext.Request.GetEncodedUrl() }
            });

        base.OnResultExecuting(context);
    }

    private static bool ConfrontarUsuario(ResultExecutingContext context)
    {
        var idUsuarioLogado = context.HttpContext.User
            .FindFirst("UsuarioId")?
            .Value;

        var routeValues = context?.HttpContext?.Request?.RouteValues;
        var id = string.Empty;
        if (routeValues.ContainsKey("id"))
            id = routeValues["id"].ToString();
        else if (routeValues.ContainsKey("usuarioId"))
            id = routeValues["usuarioId"].ToString();

        return id == idUsuarioLogado;
    }
}