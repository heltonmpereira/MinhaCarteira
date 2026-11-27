using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MinhaCarteira.AppServer.Model;

namespace MinhaCarteira.AppServer.Helper;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ValidarPosseAttribute(
    string caminhoPropriedade = "ProprietarioId",
    string nomeMetodo = "ObterPorId")
    : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        // fail-closed: se não der para provar a posse, nega
        if (ctx.Controller is not IValidacaoPosseController validacao
            || !ObterIds(ctx, out var ids))
        {
            ctx.Result = new ForbidResult();
            return;
        }

        foreach (var id in ids)
        {
            var resultado = await validacao.RegistroPertenceAoUsuarioLogadoAsync(id, caminhoPropriedade, nomeMetodo);
            if (resultado is ForbidResult)
            {
                ctx.Result = resultado;
                return;
            }
        }

        await next();   // só chega na action se tudo pertence ao usuário
    }

    // Resolve o(s) id(s) a partir dos argumentos da action:
    // id direto (Guid), entidade (propriedade Id) ou lista de qualquer um dos dois.
    private static bool ObterIds(ActionExecutingContext ctx, out List<object> ids)
    {
        ids = [];

        foreach (var argumento in ctx.ActionArguments.Values)
        {
            if (argumento is null || argumento is string)
                continue;

            if (argumento is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    var id = ObterIdDoItem(item);
                    if (id is not null) ids.Add(id);
                }
            }
            else
            {
                var id = ObterIdDoItem(argumento);
                if (id is not null) ids.Add(id);
            }
        }

        return ids.Count > 0;
    }

    private static object ObterIdDoItem(object item)
    {
        if (item is null) return null;
        if (item is Guid guid) return guid;
        return item.GetType().GetProperty("Id")?.GetValue(item);
    }
}