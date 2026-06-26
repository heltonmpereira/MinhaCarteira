using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class ConciliacaoBancariaMap : BaseMap<ConciliacaoBancaria, Guid>
{
    public override void Configure(EntityTypeBuilder<ConciliacaoBancaria> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataCadastro).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");

        builder
            .HasOne(o => o.Proprietario)
            .WithMany()
            .HasForeignKey(f => f.ProprietarioId)
            .OnDelete(DeleteBehavior.NoAction);

        /*

        builder
            .HasMany(m => m.Movimentos)
            .WithOne(o => o.ConciliacaoBancariaAgendamentoParcela)
            .HasForeignKey(f => f.ConciliacaoBancariaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(m => m.AgendamentoParcelas)
            .WithOne(o => o.ConciliacaoBancariaAgendamentoParcela)
            .HasForeignKey(f => f.ConciliacaoBancariaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(m => m.AgendamentoParcelas)
            .WithOne(o => o.ConciliacaoBancariaAgendamentoParcela)
            .HasForeignKey(f => f.ConciliacaoBancariaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(m => m.Movimentos)
            .WithOne(o => o.ConciliacaoBancariaAgendamentoParcela)
            .HasForeignKey(f => f.ConciliacaoBancariaId)
            .OnDelete(DeleteBehavior.Cascade);
        */
    }
}