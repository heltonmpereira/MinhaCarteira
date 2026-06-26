using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;

namespace MinhaCarteira.AppCliente.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : PadraoController
{
    // GET: HomeController
    public ActionResult Index()
    {
        return View();
    }
}