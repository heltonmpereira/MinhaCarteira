using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Triggers;

public class MovimentoBancarioTrigger(IDbContext dbContext)
    : IBeforeSaveTrigger<MovimentoBancario>, IAfterSaveTrigger<MovimentoBancario>
{
    public async Task BeforeSave(ITriggerContext<MovimentoBancario> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added) return;

        var itemDb = await dbContext.MovimentosBancarios
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == context.Entity.Id, cancellationToken);

        var conta = await dbContext.ContasBancarias.FindAsync(context.Entity.ContaBancariaId, cancellationToken);
        if (conta == null || conta.DataSaldoInicial > context.Entity.DataMovimento) return;

        if (itemDb == null) 
            return;

        conta.ValorSaldoAtual += itemDb.ValorReal * -1;
        dbContext.ContasBancarias.Entry(conta).Property(p => p.ValorSaldoAtual).IsModified = true;
    }

    public async Task AfterSave(ITriggerContext<MovimentoBancario> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Deleted) return;

        var conta = await dbContext.ContasBancarias.FindAsync(context.Entity.ContaBancariaId, cancellationToken);
        if (conta == null || conta.DataSaldoInicial > context.Entity.DataMovimento) return;

        dbContext.ContasBancarias.Attach(conta);
        conta.ValorSaldoAtual += context.Entity.ValorReal;
        dbContext.ContasBancarias.Entry(conta).Property(p => p.ValorSaldoAtual).IsModified = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}