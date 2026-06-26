using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Extrato;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IImportarArquivoRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<ImportarArquivoViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<ImportarArquivoViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<ImportarArquivoViewModel>> Alterar(ImportarArquivoViewModel item);

    [Post("")]
    Task<RespostaServico<ImportarArquivoViewModel>> Incluir(ImportarArquivoViewModel item);

    [Delete("/{id}")]
    Task<RespostaServico<int>> Deletar(Guid id);

    [Post("/processar-arquivo")]
    Task<RespostaPaginadaServico<MovimentoBancarioViewModel>> ProcessarArquivo(FileUploadModel model);

    [Post("/importar-movimentos")]
    Task<RespostaServico<bool>> ImportarMovimentos(List<MovimentoBancarioViewModel> itens);
}
