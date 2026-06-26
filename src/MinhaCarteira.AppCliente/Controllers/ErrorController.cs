using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinhaCarteira.AppCliente.Models;

namespace MinhaCarteira.AppCliente.Controllers;

public class ErrorController(ILogger<HomeController> logger) : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        var exceptionHandle = HttpContext.Features
            .Get<IExceptionHandlerPathFeature>();

        var viewModel = new ErrorViewModel(exceptionHandle)
        {
            Caminho = exceptionHandle.Path ?? HttpContext.Request.Path,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        logger.LogError(
            exception: exceptionHandle.Error,
            message: "Mensagem de erro: {Mensagem}",
            viewModel.Mensagem);
        return View(viewModel);
    }
}