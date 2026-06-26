using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Data;

namespace MinhaCarteira.Modelo.Triggers;

public class ConciliacaoBancariaAgendamentoParcelaTrigger(IDbContext dbContext)
    : IAfterSaveTrigger<ConciliacaoBancariaAgendamentoParcela>,
        IBeforeSaveTrigger<ConciliacaoBancariaAgendamentoParcela>
{
    private async Task<AgendamentoParcela> ObterPorId(ConciliacaoBancariaAgendamentoParcela item, CancellationToken cancellationToken)
    {
        if (dbContext == null) return null;
        var itemDb = await dbContext.AgendamentoParcelas
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == item.AgendamentoParcelaId, cancellationToken);

        dbContext.ChangeTracker.Clear();

        return itemDb;
    }

    private Task AlterarFilho(AgendamentoParcela item, Guid? id, CancellationToken cancellationToken)
    {
        item.ConciliacaoBancariaAgendamentoParcelaId = id;
        dbContext.Entry(item).Property(p => p.ConciliacaoBancariaAgendamentoParcelaId).IsModified = true;
        return Task.CompletedTask;
    }

    public async Task BeforeSave(ITriggerContext<ConciliacaoBancariaAgendamentoParcela> context, CancellationToken cancellationToken)
    {
        if (dbContext == null) return;
        if (context.ChangeType != ChangeType.Deleted) return;

        var itemDb = await ObterPorId(context.Entity, cancellationToken);
        await AlterarFilho(itemDb, null, cancellationToken);
    }

    public async Task AfterSave(ITriggerContext<ConciliacaoBancariaAgendamentoParcela> context, CancellationToken cancellationToken)
    {
        if (dbContext == null) return;
        if (context.ChangeType != ChangeType.Added) return;

        var itemDb = await ObterPorId(context.Entity, cancellationToken);
        await AlterarFilho(itemDb, context.Entity.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}