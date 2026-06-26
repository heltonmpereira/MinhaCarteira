using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class ConciliacaoBancariaRepositorio(IDbContext contexto)
    : BaseRepositorio<ConciliacaoBancaria, Guid>(contexto), IConciliacaoBancariaRepositorio
{
    protected override IQueryable<ConciliacaoBancaria> AdicionarIncludes(IQueryable<ConciliacaoBancaria> source, params object[] args)
    {
        return source
            .Include(i => i.Movimentos)
            .Include(i => i.AgendamentoParcelas);
    }

    public async Task<ConciliacaoBancaria> ObterPorParcelaId(Guid id, bool adicionarIncludes = true)
    {
        var parcela = await Contexto
            .ConciliacoesBancariasAgendamentoParcelas
            .SingleOrDefaultAsync(s => s.Id == id);

        if (parcela == null) return null;
        return await ObterPorId(parcela.ConciliacaoBancariaId);
    }

    public override async Task<int> DeletarRange(Guid[] ids)
    {
        var movimentos = new List<ConciliacaoBancariaMovimento>();
        var parcelas = new List<ConciliacaoBancariaAgendamentoParcela>();

        foreach (var id in ids)
        {
            var parcelasDb = Contexto
                .ConciliacoesBancariasAgendamentoParcelas
                .Where(w => ids.Contains(w.ConciliacaoBancariaId))
                .ToList();

            var movimentosDb = Contexto
                .ConciliacoesBancariasMovimentos
                .Where(w => ids.Contains(w.ConciliacaoBancariaId))
                .ToList();

            parcelas.AddRange(parcelasDb);
            movimentos.AddRange(movimentosDb);
        }
        Contexto.ConciliacoesBancariasMovimentos.RemoveRange(movimentos);
        Contexto.ConciliacoesBancariasAgendamentoParcelas.RemoveRange(parcelas);
        await Contexto.SaveChangesAsync().ConfigureAwait(false);

        await base.DeletarRange(ids);

        return 1;
    }
}