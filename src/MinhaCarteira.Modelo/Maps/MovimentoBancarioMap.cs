using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class MovimentoBancarioMap : BaseMap<MovimentoBancario, Guid>
{
    public override void Configure(EntityTypeBuilder<MovimentoBancario> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataMovimento).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataCriacao).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAlteracao).HasColumnType("timestamp without time zone");

        builder.Property(p => p.IdAuxiliar).HasMaxLength(200);
        builder.Property(p => p.Valor).HasPrecision(18, 6);
        builder.Property(p => p.Descricao).HasMaxLength(200);
        builder.Property(p => p.Competencia)
            .HasMaxLength(6)
            .HasDefaultValue("substring(TO_CHAR(now(), 'YYYYMMDD'), 1, 6)");
        builder.Property(p => p.TipoMovimento).HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => (TipoMovimento)Enum.Parse(typeof(TipoMovimento), v));

        builder.HasOne(o => o.Pessoa)
            .WithMany()
            .HasForeignKey(f => f.PessoaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Categoria)
            .WithMany()
            .HasForeignKey(f => f.CategoriaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.CentroClassificacao)
            .WithMany()
            .HasForeignKey(f => f.CentroClassificacaoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.ContaBancaria)
            .WithMany(m => m.Movimentos)
            .HasForeignKey(f => f.ContaBancariaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.ImportarArquivo)
            .WithMany()
            .HasForeignKey(f => f.ImportarArquivoId)
            .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasOne(o => o.ConciliacaoBancariaAgendamentoParcela)
        //    .WithMany()
        //    .HasForeignKey(f => f.ConciliacaoBancariaId)
        //    .OnDelete(DeleteBehavior.SetNull);
    }
}
