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

public class PapelRepositorio(IDbContext contexto) : BaseRepositorio<Papel, Guid>(contexto), IPapelRepositorio
{
    public async Task<Papel> AtualizarUsuarios(Guid id, Guid[] idsUsuario)
    {
        var itemDb = await ObterPorId(id);

        var itensRemovidos = new List<UsuarioPapel>();
        var itensAdicionados = new List<UsuarioPapel>();

        foreach (var userPapel in itemDb.Usuarios)
            if (!idsUsuario.Contains(userPapel.UsuarioId))
                itensRemovidos.Add(userPapel);

        foreach (var idUsuario in idsUsuario.Distinct())
            if (!itemDb.Usuarios.Any(a => a.UsuarioId == idUsuario))
                itensAdicionados.Add(new UsuarioPapel()
                {
                    PapelId = id,
                    UsuarioId = idUsuario
                });

        var tabPapeis = Contexto.Set<UsuarioPapel>();
        tabPapeis.RemoveRange(itensRemovidos);
        await tabPapeis.AddRangeAsync(itensAdicionados);

        await Contexto.SaveChangesAsync();

        return itemDb;
    }

    protected override IQueryable<Papel> AdicionarIncludes(IQueryable<Papel> source, params object[] args)
    {
        return source
            .Include(i => i.Usuarios)
                .ThenInclude(ti => ti.Usuario);
    }
}