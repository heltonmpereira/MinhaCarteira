using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Triggers;

namespace MinhaCarteira.Modelo.Data;

public class MinhaCarteiraContext(DbContextOptions<MinhaCarteiraContext> options) : DbContext(options), IDbContext
{
    private void ValidarEntidades()
    {
        var entities = from e in ChangeTracker.Entries()
                       where e.State == EntityState.Added ||
                             e.State == EntityState.Modified
                       select e.Entity;

        foreach (var entity in entities)
        {
            var validationContext = new ValidationContext(entity);
            Validator.ValidateObject(
                entity,
                validationContext,
                true);
        }
    }
    private IEnumerable<EntityEntry> EntradasComPropriedade(
        IEnumerable<string> nomes, EntityState estado)
    {
        var entries = ChangeTracker.Entries()
            .Where(w => w.State == estado && w.Entity.GetType()
                .GetProperties()
                .Any(a => nomes.Contains(a.Name)))
            .ToList();

        return entries;
    }
    private static PropertyEntry ObterPropriedade(EntityEntry obj,
        IEnumerable<string> name)
    {
        foreach (var nome in name)
        {
            try
            {
                return obj.Property(nome);
            }
            catch
            {
                //nada por aqui...
            }
        }

        return null;
    }
    private void PreparaCampos()
    {
        var camposCadastro = new[] { "DataCadastro", "Data_Cadastro", "DataCriacao" };

        var entries = EntradasComPropriedade(camposCadastro, EntityState.Added);
        foreach (var entry in entries)
        {
            var campo = ObterPropriedade(entry, camposCadastro);
            if (campo == null)
                break;

            campo.CurrentValue = DateTime.Now;
        }

        var camposAtualizacao = new[] { "DataAtualizacao", "Data_Atualizacao", "DataAlteracao" };
        var entriesAlteradas = EntradasComPropriedade(camposAtualizacao, EntityState.Modified);
        foreach (var entry in entriesAlteradas)
        {
            var campo = ObterPropriedade(entry, camposAtualizacao);
            if (campo == null)
                break;

            campo.CurrentValue = DateTime.Now;
        }

        var entriesCadastros = EntradasComPropriedade(camposCadastro, EntityState.Modified);
        foreach (var entry in entriesCadastros)
        {
            var campo = ObterPropriedade(entry, camposCadastro);
            if (campo == null)
                break;

            campo.CurrentValue = campo.OriginalValue;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(message => Debug.WriteLine(message))
            .EnableDetailedErrors()
            .UseTriggers(tgr =>
            {
                tgr.AddTrigger<CategoriaTrigger>();
                tgr.AddTrigger<MovimentoBancarioTrigger>();
                tgr.AddTrigger<ContaBancariaTrigger>();
                tgr.AddTrigger<ConciliacaoBancariaMovimentoTrigger>();
                tgr.AddTrigger<ConciliacaoBancariaAgendamentoParcelaTrigger>();
            });
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif


        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
#if DEBUG
        foreach (var entry in ChangeTracker.Entries())
            Debug.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
#endif

        ValidarEntidades();
        PreparaCampos();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> MySaveChangesWithoutTriggersAsync(CancellationToken cancellationToken = default)
    {
        ValidarEntidades();
        PreparaCampos();
        return await this.SaveChangesWithoutTriggersAsync(cancellationToken);
    }

    public DbSet<Arquivo> Arquivos { get; set; }
    public DbSet<Icone> Icones { get; set; }
    public DbSet<Papel> Papeis { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Organizacao> Organizacoes { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<CentroClassificacao> CentrosClassificacao { get; set; }
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<ContaBancaria> ContasBancarias { get; set; }
    public DbSet<MovimentoBancario> MovimentosBancarios { get; set; }
    public DbSet<MovimentoBancarioArquivo> MovimentosBancarioArquivos { get; set; }
    public DbSet<InstituicaoFinanceira> InstituicoesFinanceiras { get; set; }
    public DbSet<RegraImportacao> RegraImportacaos { get; set; }
    public DbSet<ImportarArquivo> ImportarArquivos { get; set; }
    public DbSet<TransferenciaBancaria> TransferenciasBancarias { get; set; }
    public DbSet<DashboardMonitor> DashboardMonitores { get; set; }
    public DbSet<ConciliacaoBancaria> ConciliacoesBancarias { get; set; }
    public DbSet<ConciliacaoBancariaMovimento> ConciliacoesBancariasMovimentos { get; set; }
    public DbSet<ConciliacaoBancariaAgendamentoParcela> ConciliacoesBancariasAgendamentoParcelas { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }
    public DbSet<AgendamentoParcela> AgendamentoParcelas { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }
}
