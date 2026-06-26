
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MinhaCarteira.AppServer.Helper;

public class LoggingActionFilter : IAsyncActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controllerName = context.RouteData.Values["controller"];
        var actionName = context.RouteData.Values["action"];

        _logger.LogInformation("Executando endpoint - Controller: {Controller}, Action: {Action}, Parâmetros: {@Parameters}",
            controllerName, actionName, context.ActionArguments);

        var resultContext = await next();

        _logger.LogInformation("Finalizado endpoint - Controller: {Controller}, Action: {Action}",
            controllerName, actionName);
    }
}
