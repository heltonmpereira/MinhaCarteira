using Microsoft.EntityFrameworkCore;
using MinhaCarteira.Definicao.Relatorio.FluxoCaixa;
using System.Reflection;

namespace MinhaCarteira.Modelo.Data;

public class RelatorioContext(DbContextOptions<RelatorioContext> options) : DbContext(options)
{
    //DbSets para rotinas de consulta e relatório
    public DbSet<FluxoCaixaItem> FluxoCaixaItens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<FluxoCaixaItem>().HasNoKey();
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());


        base.OnModelCreating(modelBuilder);
    }
}
