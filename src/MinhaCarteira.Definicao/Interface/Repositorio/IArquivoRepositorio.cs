using System;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;

namespace MinhaCarteira.Definicao.Interface.Repositorio
{
    public interface IArquivoRepositorio : IRepositorio<Arquivo, Guid>
    {
    }
}
