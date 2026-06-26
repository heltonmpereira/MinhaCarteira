using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class ConciliacaoBancariaAgendamentoParcelaMap : BaseMap<ConciliacaoBancariaAgendamentoParcela, Guid>
{
    public override void Configure(EntityTypeBuilder<ConciliacaoBancariaAgendamentoParcela> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(o => o.AgendamentoParcela)
            .WithOne(m => m.ConciliacaoBancariaAgendamentoParcela)
            .HasForeignKey<AgendamentoParcela>()
            .OnDelete(DeleteBehavior.NoAction);
    }
}