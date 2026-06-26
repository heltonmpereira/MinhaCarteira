using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;

namespace MinhaCarteira.Modelo.Maps;

public class TransferenciaBancariaMap : Base.BaseMap<TransferenciaBancaria, Guid>
{
    public override void Configure(EntityTypeBuilder<TransferenciaBancaria> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(o => o.Origem)
            .WithOne()
            .HasForeignKey<TransferenciaBancaria>(f => f.MovimentoOrigemId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(o => o.Destino)
            .WithOne()
            .HasForeignKey<TransferenciaBancaria>(f => f.MovimentoDestinoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}