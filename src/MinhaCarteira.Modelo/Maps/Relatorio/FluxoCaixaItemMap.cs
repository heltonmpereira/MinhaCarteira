using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Relatorio.FluxoCaixa;

namespace MinhaCarteira.Modelo.Maps.Relatorio;

internal class FluxoCaixaItemMap : IEntityTypeConfiguration<FluxoCaixaItem>
{
    public void Configure(EntityTypeBuilder<FluxoCaixaItem> builder)
    {
        builder.ToTable(nameof(FluxoCaixaItem), t => t.ExcludeFromMigrations());
        builder.HasNoKey();

        builder.Property(p => p.Marco).HasColumnName("Março");

        builder.Property(p => p.Janeiro).HasPrecision(10, 2);
        builder.Property(p => p.Fevereiro).HasPrecision(10, 2);
        builder.Property(p => p.Marco).HasPrecision(10, 2);
        builder.Property(p => p.Abril).HasPrecision(10, 2);
        builder.Property(p => p.Maio).HasPrecision(10, 2);
        builder.Property(p => p.Junho).HasPrecision(10, 2);
        builder.Property(p => p.Julho).HasPrecision(10, 2);
        builder.Property(p => p.Agosto).HasPrecision(10, 2);
        builder.Property(p => p.Setembro).HasPrecision(10, 2);
        builder.Property(p => p.Outubro).HasPrecision(10, 2);
        builder.Property(p => p.Novembro).HasPrecision(10, 2);
        builder.Property(p => p.Dezembro).HasPrecision(10, 2);
        builder.Property(p => p.Total).HasPrecision(10, 2);
    }
}
