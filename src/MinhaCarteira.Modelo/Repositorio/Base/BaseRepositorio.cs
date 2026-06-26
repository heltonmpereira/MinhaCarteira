using Dhani.Utilitarios.Filtro;
using Dhani.Utilitarios.Helper;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;
using MinhaCarteira.Modelo.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Repositorio.Base;

public abstract class BaseRepositorio<T, TPK> : IRepositorio<T, TPK>
    where T : class, IEntidade<TPK>
{
    public BaseRepositorio(IDbContext contexto)
    {
        Contexto = contexto;
        Tabela = contexto.Set<T>();
        contexto.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected IDbContext Contexto { get; }
    protected DbSet<T> Tabela { get; }

    public virtual async Task<int> Deletar(TPK id)
    {
        return await DeletarRange([id]).ConfigureAwait(false);
    }
    public virtual async Task<int> DeletarRange(TPK[] ids)
    {
        Contexto.ChangeTracker.Clear();
        var objs = Tabela.AsNoTracking().Where(w => ids.Contains(w.Id));

        Tabela.RemoveRange(objs);
        var retorno = await Contexto
            .SaveChangesAsync()
            .ConfigureAwait(false);

        return retorno;
    }

    protected virtual async Task<T> ExecutarAntesAlterar(T param)
    {
        var item = await ObterPorId(param.Id).ConfigureAwait(false);
        item.MapearIgnorandoCamposCriacaoEAlteracao(param);
        param = item;

        Contexto.Entry(item).State = EntityState.Modified;

        return param;
    }
    public virtual async Task<T> Alterar(T item)
    {
        var itemPreparado = await ExecutarAntesAlterar(item).ConfigureAwait(false);
        if (itemPreparado == null) return null;

        Tabela.Update(itemPreparado);
        await Contexto.SaveChangesAsync().ConfigureAwait(false);

        return item;
    }
    public virtual async Task<T> Incluir(T item)
    {
        await Tabela.AddAsync(item).ConfigureAwait(false);
        await Contexto.SaveChangesAsync().ConfigureAwait(false);

        return item;
    }

    protected virtual IQueryable<Tipo> AdicionarFiltro<Tipo>(
        IQueryable<Tipo> source,
        ICriterio criterio)
    {
        var filtro = criterio.CompilarFiltro<Tipo>();
        return filtro == null
            ? source
            : source.Where(filtro);
    }
    protected virtual IQueryable<T> AdicionarIncludes(
        IQueryable<T> source,
        params object[] args)
    {
        return source;
    }
    protected virtual IQueryable<T> AdicionarOrdenacao(
        IQueryable<T> source, ICriterio criterio)
    {
        if (criterio is null)
            throw new ArgumentNullException(nameof(criterio));

        if (criterio.ItensPorPagina >= 1)
        {
            if (string.IsNullOrWhiteSpace(criterio.Ordenacao))
                criterio.Ordenacao = "id";

            var ordenado = source.OrderBy(criterio.Ordenacao);
            return ordenado ?? source;
        }

        return source;
    }
    public async Task<Tuple<IList<T>, int>> Navegar(ICriterio criterio)
    {
        ArgumentNullException.ThrowIfNull(criterio);

        var tab = criterio.AdicionarIncludes
            ? AdicionarIncludes(Tabela, criterio.ParametrosInclude)
            : Tabela;

        tab = AdicionarFiltro(tab, criterio);

        var totalRegistros = await tab.CountAsync().ConfigureAwait(false);

        tab = AdicionarOrdenacao(tab, criterio);

        IList<T> itens;
        if (criterio.ItensPorPagina <= 1)
        {
            itens = await tab
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }
        else
        {
            var pagina = criterio.Pagina > 0
                ? criterio.Pagina - 1
                : 0;
            itens = await tab
                .AsNoTracking()
                .Skip(pagina * criterio.ItensPorPagina)
                .Take(criterio.ItensPorPagina)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        var retorno = new Tuple<IList<T>, int>(itens, totalRegistros);

        return retorno;
    }
    public virtual async Task<T> ObterPorId(TPK id, bool adicionarIncludes = true, params object[] args)
    {
        var item = await (adicionarIncludes
                ? AdicionarIncludes(Tabela, args)
                : Tabela)
            .AsNoTracking()
            .SingleOrDefaultAsync(s =>
                Comparer<TPK>.Default.Compare(s.Id, id) == 0);

        return item;
    }

    #region Dispose
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                Contexto.Dispose();
            }
        }
        disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}