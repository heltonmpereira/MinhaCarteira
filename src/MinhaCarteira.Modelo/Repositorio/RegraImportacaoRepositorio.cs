using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class RegraImportacaoRepositorio(IDbContext contexto)
    : BaseRepositorio<RegraImportacao, Guid>(contexto), IRegraImportacaoRepositorio
{
    protected override IQueryable<RegraImportacao> AdicionarIncludes(IQueryable<RegraImportacao> source, params object[] args)
    {
        return source
            .Include(i => i.CentroClassificacao)
            .Include(i => i.Pessoa)
            .Include(i => i.ContaBancaria)
                .ThenInclude(i => i.InstituicaoFinanceira)
                    .ThenInclude(ti => ti.Icone)
            .Include(i => i.Categoria)
                .ThenInclude(i => i.Icone);
    }
}
