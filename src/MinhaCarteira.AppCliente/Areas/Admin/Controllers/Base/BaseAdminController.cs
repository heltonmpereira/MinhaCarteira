using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.Areas.Admin.Controllers.Base;

//[Authorize(Roles = "admin,usuariologado")] 
[Area("Admin")]
public abstract class BaseAdminController<T, TPK, TSERVICO>(TSERVICO servico, IHttpContextAccessor httpContextAccessor)
    : BaseController<T, TPK, TSERVICO>(servico, httpContextAccessor)
    where T : BaseViewModel, IEntidade<TPK>
{
    [Authorize(Roles = "admin")]
    public override Task<IActionResult> Index(int page = 1)
    {
        return base.Index(page);
    }
    [Authorize(Roles = "admin")]
    public override ActionResult IndexFiltrado(CriterioViewModel model)
    {
        return base.IndexFiltrado(model);
    }
    [Authorize(Roles = "admin")]
    public override Task<IActionResult> IndexPaginado()
    {
        return base.IndexPaginado();
    }

    [Authorize(Roles = "admin")]
    public override Task<IActionResult> Alterar(TPK id)
    {
        return base.Alterar(id);
    }
    [Authorize(Roles = "admin")]
    public override Task<IActionResult> Alterar(T item)
    {
        return base.Alterar(item);
    }

    [Authorize(Roles = "admin")]
    public override Task<IActionResult> Incluir()
    {
        return base.Incluir();
    }
    [Authorize(Roles = "admin")]
    public override Task<IActionResult> Incluir(T item)
    {
        return base.Incluir(item);
    }

    [Authorize(Roles = "admin")]
    public override Task<IActionResult> Deletar(T item)
    {
        return base.Deletar(item);
    }
    [Authorize(Roles = "admin")]
    public override Task<IActionResult> Deletar(TPK id)
    {
        return base.Deletar(id);
    }

    [Authorize(Roles = "usuariologado")]
    [HttpGet]
    public override Task<JsonResult> ObterTodos(string prefix)
    {
        return base.ObterTodos(prefix);
    }
}