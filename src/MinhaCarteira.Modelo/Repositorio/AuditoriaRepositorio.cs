using System;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;
using System.Linq;

namespace MinhaCarteira.Modelo.Repositorio;

public class AuditoriaRepositorio(IDbContext contexto)
    : BaseRepositorio<Auditoria, Guid>(contexto), IAuditoriaRepositorio
{
    protected override IQueryable<Auditoria> AdicionarIncludes(IQueryable<Auditoria> source, params object[] args)
    {
        return source
            .Include(i => i.Usuario);
    }
}
