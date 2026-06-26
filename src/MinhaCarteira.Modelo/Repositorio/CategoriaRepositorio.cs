using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Repositorio;

public class CategoriaRepositorio(IDbContext contexto)
    : BaseRepositorio<Categoria, Guid>(contexto), ICategoriaRepositorio
{
    protected override IQueryable<Categoria> AdicionarIncludes(IQueryable<Categoria> source, params object[] args)
    {
        return source
            .Include(i => i.Icone)
            .Include(i => i.CategoriaPai)
                .ThenInclude(i => i.Icone);
    }

    private async Task ConfigurarRelacionamentoIcone(Categoria item)
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

    protected override async Task<Categoria> ExecutarAntesAlterar(Categoria item)
    {
        var retorno = await base.ExecutarAntesAlterar(item);
        await ConfigurarRelacionamentoIcone(item);

        return retorno;
    }
}
