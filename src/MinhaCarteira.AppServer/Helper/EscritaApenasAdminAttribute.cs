using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MinhaCarteira.AppServer.Helper;

/// <summary>
/// Exige que o usuário tenha um dos papéis informados apenas em operações de escrita
/// (POST/PUT/PATCH/DELETE). Leitura (GET/HEAD) permanece liberada para qualquer autenticado.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class EscritaApenasAdminAttribute(string papeis) : Attribute, IAsyncAuthorizationFilter
{
    public const string PapeisAdministrador = "admin,superadmin";

    private static readonly string[] MetodosEscrita = ["POST", "PUT", "PATCH", "DELETE"];

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!MetodosEscrita.Contains(context.HttpContext.Request.Method, StringComparer.OrdinalIgnoreCase))
            return Task.CompletedTask;

        var usuario = context.HttpContext.User;
        var autorizado = usuario.Identity is { IsAuthenticated: true }
            && papeis.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                     .Any(usuario.IsInRole);

        if (!autorizado)
            context.Result = new ForbidResult();

        return Task.CompletedTask;
    }
}