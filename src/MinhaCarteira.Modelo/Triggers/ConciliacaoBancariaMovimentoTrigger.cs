using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Data;

namespace MinhaCarteira.Modelo.Triggers;

public class ConciliacaoBancariaMovimentoTrigger(IDbContext dbContext)
    : IAfterSaveTrigger<ConciliacaoBancariaMovimento>, IBeforeSaveTrigger<ConciliacaoBancariaMovimento>
{
    private async Task<MovimentoBancario> ObterPorId(ConciliacaoBancariaMovimento item, CancellationToken cancellationToken)
    {
        if (dbContext == null) return null;
        var itemDb = await dbContext.MovimentosBancarios
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == item.MovimentoBancarioId, cancellationToken);

        dbContext.ChangeTracker.Clear();

        return itemDb;
    }

    private Task AlterarFilho(MovimentoBancario item, Guid? id, CancellationToken cancellationToken)
    {
        item.ConciliacaoBancariaMovimentoId = id;
        dbContext.Entry(item).Property(p => p.ConciliacaoBancariaMovimentoId).IsModified = true;
        return Task.CompletedTask;
    }

    public async Task BeforeSave(ITriggerContext<ConciliacaoBancariaMovimento> context, CancellationToken cancellationToken)
    {
        if (dbContext == null) return;
        if (context.ChangeType != ChangeType.Deleted) return;

        var itemDb = await ObterPorId(context.Entity, cancellationToken);
        await AlterarFilho(itemDb, null, cancellationToken);
    }

    public async Task AfterSave(ITriggerContext<ConciliacaoBancariaMovimento> context, CancellationToken cancellationToken)
    {
        if (dbContext == null) return;
        if (context.ChangeType != ChangeType.Added) return;

        var itemDb = await ObterPorId(context.Entity, cancellationToken);
        await AlterarFilho(itemDb, context.Entity.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}