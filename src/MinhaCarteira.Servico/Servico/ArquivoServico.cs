using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Servico.Servico.Base;
using System;

namespace MinhaCarteira.Servico.Servico
{
    public class ArquivoServico(IArquivoRepositorio repositorio)
        : BaseServico<Arquivo, Guid, IArquivoRepositorio>(repositorio), IArquivoServico;
}
