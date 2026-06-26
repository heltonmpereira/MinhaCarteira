using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MinhaCarteira.Definicao.Entidade;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MinhaCarteira.Modelo.Data;

public interface IDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    EntityEntry Attach(object entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> MySaveChangesWithoutTriggersAsync(CancellationToken cancellationToken = default);

    ChangeTracker ChangeTracker { get; }

    public DatabaseFacade Database { get; }

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
}