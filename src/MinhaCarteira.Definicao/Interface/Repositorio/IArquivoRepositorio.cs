using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaCarteira.Definicao.Interface.Repositorio
{
    public interface IArquivoRepositorio : IRepositorio<Arquivo, Guid>
    {
    }
}
