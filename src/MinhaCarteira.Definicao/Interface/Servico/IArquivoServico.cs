using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhaCarteira.Definicao.Interface.Servico
{
    public interface IArquivoServico : IServico<Arquivo, Guid, IArquivoRepositorio>
    {
    }
}
