using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Repositorio;

public class InstituicaoFinanceiraRepositorio(IDbContext contexto)
    : BaseRepositorio<InstituicaoFinanceira, Guid>(contexto), IInstituicaoFinanceiraRepositorio
{
    protected override IQueryable<InstituicaoFinanceira> AdicionarIncludes(IQueryable<InstituicaoFinanceira> source, params object[] args)
    {
        return source
            .Include(i => i.Icone);
    }

    private async Task ConfigurarRelacionamentoIcone(InstituicaoFinanceira item)
    {
        if (item.Icone != null && item.Icone.Id == Guid.Empty)
        {
            await Contexto.Icones.AddAsync(item.Icone);
            item.IconeId = item.Icone.Id;
        }
        else if (item.Icone != null && item.Icone.Id != Guid.Empty)
        {
            Contexto.Entry(item.Icone).State = EntityState.Unchanged;
            item.IconeId = item.Icone.Id;
        }
    }

    protected override async Task<InstituicaoFinanceira> ExecutarAntesAlterar(InstituicaoFinanceira item)
    {
        var retorno = await base.ExecutarAntesAlterar(item);
        await ConfigurarRelacionamentoIcone(item);

        return retorno;
    }
}