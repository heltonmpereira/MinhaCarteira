using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class AgendamentoRepositorio(IDbContext contexto)
    : BaseRepositorio<Agendamento, Guid>(contexto), IAgendamentoRepositorio
{
    public async Task AtualizarAgendamento(Agendamento agend)
    {
        Contexto.ChangeTracker.Clear();

        var existingAgendamento = await Contexto.Agendamentos
            .Include(a => a.Parcelas)
            .FirstOrDefaultAsync(a => a.Id == agend.Id);

        if (existingAgendamento != null)
        {
            var parcelasNovas = agend.Parcelas.Where(w => w.Id == Guid.Empty).ToList();
            var parcelasExistentes = agend.Parcelas.Where(w => w.Id != Guid.Empty).ToList();

            foreach (var newParcela in parcelasNovas)
            {
                existingAgendamento.Parcelas.Add(newParcela);
                Contexto.Entry(newParcela).State = EntityState.Added;
            }
            foreach (var newParcela in parcelasExistentes)
            {
                Contexto.AgendamentoParcelas.Update(newParcela);
            }
        }

        await Contexto.MySaveChangesWithoutTriggersAsync();
    }

    protected override IQueryable<Agendamento> AdicionarIncludes(IQueryable<Agendamento> source, params object[] args)
    {
        source = source
            .Include(i => i.Pessoa)
            .Include(i => i.ContaBancaria)
            .Include(ti => ti.CentroClassificacao)
            .Include(ti => ti.Pessoa)
            .Include(ti => ti.ContaBancaria)
            .Include(ti => ti.Categoria)
                .ThenInclude(ti => ti.Icone)
            .Include(ti => ti.Categoria)
                .ThenInclude(ti => ti.CategoriaPai)
                .ThenInclude(ti => ti.CategoriaPai);

        if (args != null && args.Length > 0 && args.Any(a => a.ToString().Contains("incluirParcelas")))
        {
            source = source.Include(i => i.Parcelas);
        }

        return source;
    }
}
