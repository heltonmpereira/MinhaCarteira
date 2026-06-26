using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps;

public class DashboardMonitorMap : BaseMap<DashboardMonitor, Guid>
{
    public override void Configure(EntityTypeBuilder<DashboardMonitor> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.Titulo).HasMaxLength(150).IsRequired();
        builder.Property(p => p.CorFundo).HasMaxLength(150).IsRequired();
        builder.Property(p => p.DataCadastro).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAtualizacao).HasColumnType("timestamp without time zone");

        builder.HasOne(o => o.Agendamento)
            .WithMany()
            .HasForeignKey(f => f.AgendamentoId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
