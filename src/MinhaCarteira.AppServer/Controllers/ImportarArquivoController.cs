using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Modelo.Extrato;

namespace MinhaCarteira.AppServer.Controllers;

public class ImportarArquivoController(IImportarArquivoServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<ImportarArquivo, Guid, IImportarArquivoServico, IImportarArquivoRepositorio>(servico,
        httpContextAccessor)
{
    [HttpPost("processar-arquivo")]
    public async Task<IActionResult> ProcessarArquivo(FileUploadModel model)
    {
        model.ProprietarioId = Guid.Parse(IdUsuarioLogado);
        return await Servico.RespostaPaginadaServicoAsync<MovimentoBancario>(
            parameters: [model]);
    }

    [HttpPost("importar-movimentos")]
    public async Task<IActionResult> ImportarMovimentos(List<MovimentoBancario> itens) =>
        await Servico.RespostaServicoAsync<bool>(
            parameters: [itens]);
}
