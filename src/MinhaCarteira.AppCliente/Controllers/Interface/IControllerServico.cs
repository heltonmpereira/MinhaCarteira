using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MinhaCarteira.AppCliente.Controllers.Interface;

public interface IControllerServico<TSERVICO>
{
    TSERVICO Servico { get; }
    ITempDataDictionary TempData { get; set; }
    HttpContext HttpContext { get; }
    dynamic ViewBag { get; }
    ControllerContext ControllerContext { get; set; }

    ViewResult View();
    ViewResult View(string viewName);
    ViewResult View(object model);
    ViewResult View(string viewName, object model);

    Task InicializarObjeto(object instancia);

    RedirectToActionResult RedirectToAction();
    RedirectToActionResult RedirectToAction(string actionName);
    RedirectToActionResult RedirectToAction(string actionName, object routeValues);
    RedirectToActionResult RedirectToAction(string actionName, string controllerName);
    RedirectToActionResult RedirectToAction(
        string actionName,
        string controllerName,
        object routeValues);
    RedirectToActionResult RedirectToAction(
        string actionName,
        string controllerName,
        string fragment);
    RedirectToActionResult RedirectToAction(
        string actionName,
        string controllerName,
        object routeValues,
        string fragment);
}