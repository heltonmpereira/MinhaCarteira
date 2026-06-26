using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class ImportarArquivoMap : BaseMap<ImportarArquivo, Guid>
{
    public override void Configure(EntityTypeBuilder<ImportarArquivo> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataCriacao).HasColumnType("timestamp without time zone");

        builder.HasOne(o => o.ContaBancaria)
            .WithMany()
            .HasForeignKey(f => f.ContaBancariaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Proprietario)
            .WithMany()
            .HasForeignKey(f => f.ProprietarioId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
