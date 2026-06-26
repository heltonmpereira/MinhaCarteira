using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Controllers;

public class CategoriaPessoalController(ICategoriaRefit servico, IHttpContextAccessor httpContextAccessor)
    : BaseController<CategoriaViewModel, Guid, ICategoriaRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "Caminho";

    protected override CategoriaViewModel ExecutarAntesSalvar(CategoriaViewModel item)
    {
        item.ProprietarioId = IdUsuarioLogado;

        if (!(item.Icone.IconeForm?.Length > 0))
            return item;

        using var ms = new MemoryStream();
        item.Icone.IconeForm.CopyTo(ms);
        var fileBytes = ms.ToArray();
        item.Icone.ProprietarioId = IdUsuarioLogado;
        item.Icone.Nome = item.Icone.IconeForm.FileName;
        item.Icone.Conteudo = Convert.ToBase64String(fileBytes);
        item.Icone.Extensao = Path.GetExtension(item.Icone.IconeForm.FileName);

        return item;
    }

    private IActionResult ValidarProprietarioCategoria(IActionResult action)
    {
        if (action is not ViewResult result) return action;

        var item = result.Model as CategoriaViewModel;
        return item?.ProprietarioId == IdUsuarioLogado
            ? action
            : View(new CategoriaViewModel());
    }

    public override async Task<IActionResult> Deletar(Guid id)
    {
        return ValidarProprietarioCategoria(await base.Deletar(id));
    }
}
