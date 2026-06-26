using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class PessoaRepositorio(IDbContext contexto) : BaseRepositorio<Pessoa, Guid>(contexto), IPessoaRepositorio
{
    protected override IQueryable<Pessoa> AdicionarIncludes(IQueryable<Pessoa> source, params object[] args)
    {
        return source
            .Include(i => i.CentroClassificacao)
            .Include(i => i.Categoria)
                .ThenInclude(i => i.Icone);
    }
}
