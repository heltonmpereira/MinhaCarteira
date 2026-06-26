using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;

namespace MinhaCarteira.Definicao.Interface.Servico.Base;

public interface IServico<T, TPK, TRepositorio>
    where T : class, IEntidade<TPK>
    where TRepositorio : IRepositorio<T, TPK>
{
    TRepositorio Repositorio { get; }

    Task<IRespostaServico<int>> DeletarRange(TPK[] ids);
    Task<IRespostaServico<int>> Deletar(TPK id);
    Task<IRespostaServico<T>> Alterar(T item);
    Task<IRespostaPaginadaServico<T>> Navegar(ICriterio criterio);
    Task<IRespostaServico<T>> Incluir(T item);
    Task<IRespostaServico<T>> ObterPorId(TPK id, bool adicionarIncludes = true);
}