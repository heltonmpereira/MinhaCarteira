using System;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;

namespace MinhaCarteira.Definicao.Interface.Repositorio;

public interface IConciliacaoBancariaRepositorio : IRepositorio<ConciliacaoBancaria, Guid>
{
    Task<ConciliacaoBancaria> ObterPorParcelaId(Guid id, bool adicionarIncludes = true);
}