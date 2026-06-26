using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class ConciliacaoBancariaMovimentoMap : BaseMap<ConciliacaoBancariaMovimento, Guid>
{
    public override void Configure(EntityTypeBuilder<ConciliacaoBancariaMovimento> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(o => o.MovimentoBancario)
            .WithOne(m => m.ConciliacaoBancaria)
            .HasForeignKey<MovimentoBancario>()
            .OnDelete(DeleteBehavior.SetNull);
    }
}