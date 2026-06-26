using Dhani.Utilitarios.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Helper;
using MinhaCarteira.AppCliente.Models.Interface.Resposta;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;
using MinhaCarteira.AppCliente.Models.Extrato;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class ExtraImportarArquivoController(IImportarArquivoRefit servico, IHttpContextAccessor httpContextAccessor)
    : BaseController<ImportarArquivoViewModel, Guid, IImportarArquivoRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "DataCriacao desc";

    [HttpPost]
    public async Task<JsonResult> ImportarMovimentos(
        [FromBody] List<MovimentoBancarioViewModel> itens)
    {
        var view = await this.ChamarServicoProsseguirIndex<List<MovimentoBancarioViewModel>, Guid, IImportarArquivoRefit>(itens);

        return Json(view);
    }

    [HttpPost]
    public async Task<JsonResult> ProcessarArquivos(string idConta)
    {
        try
        {
            var file = HttpContext.Request.Form.Files[0];
            switch (file)
            {
                case { Length: > 0 }:
                    {
                        using var stream = new MemoryStream();
                        await file.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        var model = new FileUploadModel
                        {
                            ContaBancariaId = new Guid(idConta),
                            Conteudo = stream.ToArray(),
                            NomeArquivo = file.FileName
                        };

                        var retorno = await this.ChamarServicoView<
                            IRespostaPaginadaServico<MovimentoBancarioViewModel>,
                            FileUploadModel,
                            Guid,
                            IImportarArquivoRefit>(
                            model,
                            false,
                            nameof(IImportarArquivoRefit.ProcessarArquivo),
                            "",
                            "");

                        // Verifique se a solicitação foi bem-sucedida
                        if (retorno is not ViewResult view)
                            return Json(new { success = false, message = $"Erro ao enviar o arquivo para a API:" });

                        var itens = view.Model as List<MovimentoBancarioViewModel>;
                        itens = itens?.OrderBy(o => o.DataMovimento).ToList();

                        return Json(itens);

                    }
                default:
                    return Json(new { success = false, message = "Selecione um arquivo para enviar." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Erro ao enviar o arquivo: {ex.Message}" });
        }
    }

}
