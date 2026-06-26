using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class AgendamentoParcelaMap : BaseMap<AgendamentoParcela, Guid>
{
    public override void Configure(EntityTypeBuilder<AgendamentoParcela> builder)
    {
        base.Configure(builder);
        //builder.Property(p => p.EstahPaga).HasDefaultValue(false);
        //builder.Property(p => p.EstahConciliada).HasDefaultValue(false);
        builder.Property(p => p.EstahPaga).HasComputedColumnSql("CAST(CASE WHEN \"DataPagamento\" IS NULL THEN 0 ELSE 1 END AS boolean)", stored: true);
        builder.Property(p => p.EstahConciliada).HasComputedColumnSql("CAST(CASE WHEN \"ConciliacaoBancariaAgendamentoParcelaId\" IS NULL THEN 0 ELSE 1 END AS boolean)", stored: true);
        builder.Property(p => p.Valor).HasPrecision(18, 6);
        builder.Property(p => p.ValorPago).HasPrecision(18, 6);
        builder.Property(p => p.Data).HasPrecision(0).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataPagamento).HasPrecision(0).HasColumnType("timestamp without time zone");
        builder.Property(p => p.NumeroParcela).HasDefaultValueSql("1");

        builder.HasOne(o => o.Agendamento)
            .WithMany(m => m.Parcelas)
            .HasForeignKey(f => f.AgendamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Pessoa)
            .WithMany()
            .HasForeignKey(f => f.PessoaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.ContaBancaria)
            .WithMany()
            .HasForeignKey(f => f.ContaBancariaId)
            .OnDelete(DeleteBehavior.NoAction);

        //builder.HasMany(m => m.Movimentos)
        //    .WithOne(o => o.AgendamentoParcela)
        //    .OnDelete(DeleteBehavior.NoAction);

    }
}
