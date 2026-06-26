using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.Servico.Servico.Base;

public abstract class BaseServico<T, TPK, TRepositorio>(TRepositorio repositorio) : IServico<T, TPK, TRepositorio>
    where T : class, IEntidade<TPK>
    where TRepositorio : IRepositorio<T, TPK>
{
    public TRepositorio Repositorio { get; } = repositorio;

    public virtual async Task<IRespostaServico<int>> Deletar(TPK id)
    {
        return await DeletarRange([id]).ConfigureAwait(false);
    }
    public virtual async Task<IRespostaServico<int>> DeletarRange(TPK[] ids)
    {
        var linhasAfetadas = await Repositorio.DeletarRange(ids).ConfigureAwait(false);
        var retorno = new RespostaServico<int>(linhasAfetadas)
        {
            BemSucedido = linhasAfetadas > 0,
            Mensagem = linhasAfetadas > 0
                ? $"{linhasAfetadas} registro removido com sucesso."
                : "Falha ao remover os registros solicitados."
        };

        return retorno;
    }
    public virtual async Task<IRespostaServico<T>> Alterar(T item)
    {
        var itemDb = await Repositorio.Alterar(item).ConfigureAwait(false);
        var retorno = new RespostaServico<T>(item)
        {
            BemSucedido = itemDb != null,
            Mensagem = itemDb != null
                ? "Registro alterado com sucesso."
                : "Falha ao alterar o registro solicitado."
        };

        return retorno;
    }
    public virtual async Task<IRespostaServico<T>> Incluir(T item)
    {
        var itemDb = await Repositorio.Incluir(item).ConfigureAwait(false);
        var retorno = new RespostaServico<T>(item)
        {
            BemSucedido = itemDb != null,
            Mensagem = itemDb != null
                ? "Registro cadastrado com sucesso."
                : "Falha ao cadastrar o registro solicitado."
        };

        return retorno;
    }
    public virtual async Task<IRespostaServico<T>> ObterPorId(TPK id, bool adicionarIncludes = true)
    {
        var item = await Repositorio.ObterPorId(id).ConfigureAwait(false);
        var retorno = new RespostaServico<T>(item)
        {
            BemSucedido = item != null,
            Mensagem = item != null
               ? "Registro localizado com sucesso."
               : "Falha ao localizar o registro solicitado."
        };

        return retorno;
    }

    protected IRespostaPaginadaServico<TP> MontarRespostaPaginada<TP>(Tuple<IList<TP>, int> dados, ICriterio criterio)
    {
        var bemSucedido = dados.Item1 != null;
        var retorno = new RespostaPaginadaServico<TP>(
            dados.Item1,
            criterio.Pagina,
            criterio.ItensPorPagina,
            dados.Item2)
        {
            BemSucedido = bemSucedido,
            Mensagem = bemSucedido
                ? "Itens localizados com sucesso."
                : "Nenhum registro localizado."
        };

        if (criterio.ItensPorPagina > 1)
            retorno.TotalPaginas = (retorno.TotalRegistros / retorno.ItensPorPagina) + 1;
        else
        {
            retorno.TotalPaginas = 1;
            retorno.NumeroPagina = 1;
            retorno.ItensPorPagina = retorno.TotalRegistros == 0 ? 1 : retorno.TotalRegistros;
        }

        return retorno;
    }

    public virtual async Task<IRespostaPaginadaServico<T>> Navegar(ICriterio criterio)
    {
        var itens = await Repositorio.Navegar(criterio).ConfigureAwait(false);
        return MontarRespostaPaginada(itens, criterio);
    }

    #region Dispose
    private bool disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                Repositorio.Dispose();
            }
        }
        disposed = true;
    }
    #endregion
}