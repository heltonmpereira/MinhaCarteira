using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MinhaCarteira.Definicao.Interface.Servico;
using Newtonsoft.Json;

namespace MinhaCarteira.AppServer.Helper;

public class AuditoriaActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        
        // Only log successful actions
        if (resultContext.Exception != null)
            return;

        var httpContext = context.HttpContext;
        var registroAuditoriaServico = httpContext.RequestServices.GetRequiredService<IRegistroAuditoriaServico>();

        // Get user info
        var usuarioIdClaim = httpContext.User.FindFirst("UsuarioId")?.Value;
        var organizacaoIdClaim = httpContext.User.FindFirst("OrganizacaoId")?.Value;
        
        var usuarioId = string.IsNullOrEmpty(usuarioIdClaim) ? (Guid?)null : Guid.Parse(usuarioIdClaim);
        Guid? organizacaoId = string.IsNullOrEmpty(organizacaoIdClaim) ? null : Guid.Parse(organizacaoIdClaim);

        // Get request info
        var acao = context.HttpContext.Request.Method;
        var entidade = context.Controller.GetType().Name.Replace("Controller", "");
        var rotina = $"{context.RouteData.Values["controller"]}/{context.RouteData.Values["action"]}";
        var ipUsuario = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();

        // Get entity ID if available
        var entidadeId = context.RouteData.Values["id"]?.ToString();

        // Get request data
        string dadosNovos = null;
        if (context.ActionArguments.Count > 0)
        {
            // Don't log password in plain text!
            var argsToLog = context.ActionArguments.Values.FirstOrDefault();
            if (argsToLog != null)
            {
                var argsType = argsToLog.GetType();
                var passwordProp = argsType.GetProperty("Password") ?? argsType.GetProperty("SenhaAtual") ?? argsType.GetProperty("NovaSenha");
                if (passwordProp != null)
                {
                    // Create a copy and redact password
                    var copy = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(argsToLog));
                    if (copy != null)
                    {
                        if (copy.Password != null) copy.Password = "***REDACTED***";
                        if (copy.SenhaAtual != null) copy.SenhaAtual = "***REDACTED***";
                        if (copy.NovaSenha != null) copy.NovaSenha = "***REDACTED***";
                        dadosNovos = JsonConvert.SerializeObject(copy);
                    }
                }
                else
                {
                    dadosNovos = JsonConvert.SerializeObject(argsToLog);
                }
            }
        }

        // If we have organizacaoId, log it now
        if (organizacaoId.HasValue)
        {
            await registroAuditoriaServico.RegistrarAuditoriaAsync(
                acao,
                entidade,
                entidadeId,
                null,
                dadosNovos,
                ipUsuario,
                userAgent,
                rotina,
                usuarioId,
                organizacaoId.Value);
        }
    }
}
