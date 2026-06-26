using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Interface.Repositorio.Base;

public interface IRepositorio<T, TPK> : IDisposable
    where T : class, IEntidade<TPK>
{
    Task<int> DeletarRange(TPK[] ids);
    Task<int> Deletar(TPK id);
    Task<T> Alterar(T item);
    Task<Tuple<IList<T>, int>> Navegar(ICriterio criterio);
    Task<T> Incluir(T item);
    Task<T> ObterPorId(TPK id, bool adicionarIncludes = true, params object[] args);
}