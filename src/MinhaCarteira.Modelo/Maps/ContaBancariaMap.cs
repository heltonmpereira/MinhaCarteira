using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class ContaBancariaMap : BaseMap<ContaBancaria, Guid>
{
    public override void Configure(EntityTypeBuilder<ContaBancaria> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.DataSaldoInicial).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataCriacao).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAlteracao).HasColumnType("timestamp without time zone");

        builder.Property(p => p.Ordem).HasDefaultValue(0);
        builder.Property(p => p.Nome).HasMaxLength(200);
        builder.Property(p => p.Conta).HasMaxLength(50);
        builder.Property(p => p.Agencia).HasMaxLength(50);
        builder.Property(p => p.ValorSaldoAtual).HasPrecision(18, 6);
        builder.Property(p => p.ValorSaldoInicial).HasPrecision(18, 6);

        builder
            .HasOne(h => h.InstituicaoFinanceira)
            .WithMany()
            .HasForeignKey(f => f.InstituicaoFinanceiraId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
