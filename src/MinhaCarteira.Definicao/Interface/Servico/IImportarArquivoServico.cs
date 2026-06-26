using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo.Extrato;

namespace MinhaCarteira.Definicao.Interface.Servico;

public interface IImportarArquivoServico : IServico<ImportarArquivo, Guid, IImportarArquivoRepositorio>
{
    Task<IRespostaPaginadaServico<MovimentoBancario>> ProcessarArquivo(FileUploadModel model);
    Task<IRespostaServico<bool>> ImportarMovimentos(List<MovimentoBancario> itens);
}
