using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Repositorio;

public class ContaBancariaRepositorio(IDbContext contexto)
    : BaseRepositorio<ContaBancaria, Guid>(contexto), IContaBancariaRepositorio
{
    protected override IQueryable<ContaBancaria> AdicionarIncludes(IQueryable<ContaBancaria> source, params object[] args)
    {
        return source.Include(i => i.InstituicaoFinanceira).ThenInclude(ti => ti.Icone);
    }

    public async Task<bool> AlterarOrdemContaBancaria(Guid id, int direcao)
    {
        var itemA = await ObterPorId(id);
        var itemB = Contexto.ContasBancarias
            .Where(w => w.ProprietarioId == itemA.ProprietarioId)
            .FirstOrDefault(w => (direcao == 1
                ? w.Ordem == itemA.Ordem - 1
                : w.Ordem == itemA.Ordem + 1));

        var ordemAux = itemA.Ordem;
        itemA.Ordem = itemB?.Ordem ?? 1;
        Contexto.Entry(itemA).Property(p => p.Ordem).IsModified = true;

        if (itemB != null)
        {
            itemB.Ordem = ordemAux;
            Contexto.Entry(itemB).Property(p => p.Ordem).IsModified = true;
        }

        await Contexto.SaveChangesAsync();

        return await Task.FromResult(true);
    }

    public async Task<bool> AtualizarSaldos()
    {
        var atualizaDb = false;
        var contas = await Contexto.ContasBancarias
            .Include(i => i.Movimentos)
            .Select(s => new
            {
                ContaId = s.Id,
                SaldoInicial = s.ValorSaldoInicial,
                Movimentos = s.Movimentos.Where(w => w.DataMovimento > s.DataSaldoInicial)
            })
            .ToListAsync();

        foreach (var item in contas)
        {
            var saldo = item.SaldoInicial + item.Movimentos.Sum(s => s.ValorReal);
            var contaDb = await Contexto.ContasBancarias.FindAsync(item.ContaId);
            if (contaDb == null || contaDb.ValorSaldoAtual == saldo) continue;

            atualizaDb = true;
            contaDb.ValorSaldoAtual = saldo;
            Contexto.Entry(contaDb).Property(p => p.ValorSaldoAtual).IsModified = true;
        }

        if (atualizaDb)
            await Contexto.SaveChangesAsync();

        return true;
    }

    public async Task<int> RemoverContaBancaria(Guid id)
    {
        var contaDb = await Tabela.FindAsync(id).ConfigureAwait(false);
        if (!await Contexto.MovimentosBancarios.AnyAsync(w => w.ContaBancariaId == id).ConfigureAwait(false)
         && !await Contexto.AgendamentoParcelas.AnyAsync(w => w.ContaBancariaId == id).ConfigureAwait(false))
        {
            Contexto.ContasBancarias.Remove(contaDb);
        }
        else
        {
            contaDb.Deletado = true;

            Contexto.Entry(contaDb).Property(p => p.Deletado).IsModified = true;
        }
        return 1;
    }

    public override async Task<int> DeletarRange(Guid[] ids)
    {
        var removidos = 0;
        foreach (var id in ids)
            removidos += await RemoverContaBancaria(id);

        await Contexto.SaveChangesAsync().ConfigureAwait(false);

        return removidos;
    }

    public async Task<bool> Reativar(Guid id)
    {
        var contaDb = await Contexto.ContasBancarias.FindAsync(id);
        if (contaDb == null)
            return false;

        contaDb.Deletado = false;
        Contexto.Entry(contaDb).Property(p => p.Deletado).IsModified = true;
        var alterados = await Contexto.SaveChangesAsync().ConfigureAwait(false);

        return alterados > 0;
    }
}
