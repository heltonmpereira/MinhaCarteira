using System;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;
using System.Linq;

namespace MinhaCarteira.Modelo.Repositorio;

public class LogRepositorio(IDbContext contexto)
    : BaseRepositorio<Log, Guid>(contexto), ILogRepositorio
{
    protected override IQueryable<Log> AdicionarIncludes(IQueryable<Log> source, params object[] args)
    {
        return source
            .Include(i => i.Usuario);
    }
}
