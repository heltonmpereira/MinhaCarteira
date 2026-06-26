using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class AgendamentoParcelaRepositorio(IDbContext contexto)
    : BaseRepositorio<AgendamentoParcela, Guid>(contexto), IAgendamentoParcelaRepositorio
{
    protected override IQueryable<AgendamentoParcela> AdicionarIncludes(IQueryable<AgendamentoParcela> source, params object[] args)
    {
        return source
            .Include(i => i.Pessoa)
            .Include(i => i.ContaBancaria)
            .Include(i => i.Agendamento)
                .ThenInclude(ti => ti.CentroClassificacao)
            .Include(i => i.Agendamento)
                .ThenInclude(ti => ti.Pessoa)
            .Include(i => i.Agendamento)
                .ThenInclude(ti => ti.ContaBancaria)
            .Include(i => i.Agendamento)
                .ThenInclude(ti => ti.Categoria)
                    .ThenInclude(ti => ti.Icone)
            .Include(i => i.Agendamento)
                .ThenInclude(ti => ti.Categoria)
                    .ThenInclude(ti => ti.CategoriaPai)
                        .ThenInclude(ti => ti.CategoriaPai)
            .Include(i => i.ConciliacaoBancariaAgendamentoParcela)
                .ThenInclude(ti => ti.ConciliacaoBancaria)
                    .ThenInclude(ti => ti.Movimentos)
                        .ThenInclude(ti => ti.MovimentoBancario);
    }

    public async Task<bool> ConciliarParcelasAPartirDashboardMonitor()
    {
        return await Task.FromResult(false);
    }

    public async Task<bool> BaixarParcela(AgendamentoParcela parcela)
    {
        //Contexto.AgendamentoParcelas.Update(parcela);
        Contexto.Entry(parcela).Property(p => p.ValorPago).IsModified = true;
        Contexto.Entry(parcela).Property(p => p.DataPagamento).IsModified = true;
        return await Contexto.MySaveChangesWithoutTriggersAsync() > 0;
    }
}
