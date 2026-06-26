using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Data;

namespace MinhaCarteira.Modelo.Triggers;

public class CategoriaTrigger(IDbContext dbContext) : IAfterSaveTrigger<Categoria>
{
    private async Task<Categoria> ObterCategoria(Guid categoriaId, CancellationToken cancellationToken)
    {
        var categDb = await dbContext.Categorias
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == categoriaId, cancellationToken);

        if (categDb.CategoriaPaiId != null)
            categDb.CategoriaPai = await ObterCategoria((Guid)categDb.CategoriaPaiId, cancellationToken);

        return categDb;
    }

    public async Task AfterSave(ITriggerContext<Categoria> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Deleted) return;

        var categsAlteradas = new List<Categoria>();
        var categsDb = await dbContext.Categorias
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var item in categsDb)
        {
            var categ = await ObterCategoria(item.Id, cancellationToken);
            if (categ.CaminhoRecursivo == item.Caminho) continue;

            item.Caminho = categ.CaminhoRecursivo;
            categsAlteradas.Add(item);
        }

        if (categsAlteradas.Any())
        {
            dbContext.ChangeTracker.Clear();
            categsAlteradas.ForEach(f => dbContext.Entry(f).Property(p => p.Caminho).IsModified = true);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}