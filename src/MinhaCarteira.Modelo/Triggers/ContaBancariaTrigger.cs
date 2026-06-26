using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Data;

namespace MinhaCarteira.Modelo.Triggers;

public class ContaBancariaTrigger : IAfterSaveTrigger<ContaBancaria>
{
    private readonly IDbContext _dbContext;

    public ContaBancariaTrigger(IDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    private Task<List<ContaBancaria>> RemoverPulosOrdemContasAsync(
        IList<ContaBancaria> contas,
        CancellationToken cancellationToken)
    {
        var contasOrdenadas = contas
            .Where(w => w.Ordem != 0)
            .OrderBy(o => o.Ordem)
            .ThenBy(t => t.DataCriacao)
            .ToList();

        var contasAtualizadas = false;
        if (!contasOrdenadas.Any())
            return Task.FromResult(new List<ContaBancaria>());

        var indice = 0;
        foreach (var item in contasOrdenadas)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromResult(new List<ContaBancaria>());

            indice++;
            if (item.Ordem == indice)
                continue;

            item.Ordem = indice;
            contasAtualizadas = true;
        }

        var retorno = contasAtualizadas
            ? contasOrdenadas
            : new List<ContaBancaria>();

        return Task.FromResult(retorno);
    }
    private Task<List<ContaBancaria>> OrdenarContasSemOrdemAsync(
        IList<ContaBancaria> contas,
        CancellationToken cancellationToken)
    {
        var contasSemOrdem = contas
            .Where(w => w.Ordem == 0)
            .OrderBy(o => o.DataCriacao)
            .ToList();

        if (!contasSemOrdem.Any())
            return Task.FromResult(new List<ContaBancaria>());

        var ultimo = contas.Max(m => m.Ordem);
        foreach (var conta in contasSemOrdem)
            if (!cancellationToken.IsCancellationRequested)
                conta.Ordem = ++ultimo;

        return Task.FromResult(contasSemOrdem);
    }

    public async Task AfterSave(ITriggerContext<ContaBancaria> context, CancellationToken cancellationToken)
    {
        var contasBancarias = await _dbContext.ContasBancarias
            .Where(w => w.ProprietarioId == context.Entity.ProprietarioId)
            .ToListAsync(cancellationToken);

        _dbContext.ChangeTracker.Clear();

        if (context.ChangeType == ChangeType.Deleted)
            contasBancarias.Remove(context.Entity);

        var contasOrdenadas = await RemoverPulosOrdemContasAsync(
            contasBancarias,
            cancellationToken);
        var contasSemOrdem = await OrdenarContasSemOrdemAsync(
            contasBancarias,
            cancellationToken);

        if (context.Entity.Ordem == 0)
            context.Entity.Ordem =
                contasBancarias.Max(m => m.Ordem) + 1;

        switch (context.ChangeType)
        {
            case ChangeType.Added:
                context.Entity.ValorSaldoAtual = context.Entity.ValorSaldoInicial;
                break;
            case ChangeType.Modified:
                if (context.UnmodifiedEntity?.ValorSaldoAtual == context.Entity.ValorSaldoAtual)
                    context.Entity.ValorSaldoAtual = context.Entity.ValorSaldoInicial;
                break;
            case ChangeType.Deleted:
                break;
            default:
                break;
        }

        foreach (var conta in contasOrdenadas)
            _dbContext
                .Entry(conta)
                .Property(p => p.Ordem)
                .IsModified = true;
        
        foreach (var conta in contasSemOrdem)
            _dbContext
                .Entry(conta)
                .Property(p => p.Ordem)
                .IsModified = true;

        await _dbContext.SaveChangesAsync();
    }
}
