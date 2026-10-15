using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Modelo.Data;
using MinhaCarteira.Modelo.Repositorio.Base;

namespace MinhaCarteira.Modelo.Repositorio;

public class ImportarArquivoRepositorio(IDbContext contexto)
    : BaseRepositorio<ImportarArquivo, Guid>(contexto), IImportarArquivoRepositorio
{
    protected override IQueryable<ImportarArquivo> AdicionarIncludes(IQueryable<ImportarArquivo> source, params object[] args)
    {
        return source
            .Include(i => i.ContaBancaria);
    }
}
