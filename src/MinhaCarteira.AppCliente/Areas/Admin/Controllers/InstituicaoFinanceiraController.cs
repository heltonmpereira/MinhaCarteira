using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Areas.Admin.Controllers;

public class InstituicaoFinanceiraController(
    IInstituicaoFinanceiraRefit servico,
    IHttpContextAccessor httpContextAccessor)
    : Base.BaseAdminController<InstituicaoFinanceiraViewModel, Guid, IInstituicaoFinanceiraRefit>(servico,
        httpContextAccessor)
{
    protected override string COLUNA_ICONE_OBTER_TODOS { get; set; } = "Icone.Conteudo";
    protected override string COLUNA_ICONE_MIME_OBTER_TODOS { get; set; } = "Icone.MimeType";

    protected override InstituicaoFinanceiraViewModel ExecutarAntesSalvar(InstituicaoFinanceiraViewModel item)
    {
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
}
