using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class PapelMap : BaseMap<Papel, Guid>
{
    public override void Configure(EntityTypeBuilder<Papel> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Nome).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Observacao).HasMaxLength(500);
        builder.Property(p => p.DataCadastro).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAtualizacao).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");

        builder.HasIndex(p => p.Nome).IsUnique();

        builder.HasData(ObterDadosIniciais());
    }

    private static Papel[] ObterDadosIniciais()
    {
        var papeis = new Papel[]
        {
            new()
            {
                Id = Guid.Parse("7e0a9201-f672-11ed-9ae1-0fc4e648d876"),
                Nome = "Admin",
                DataCadastro = new DateTime(2023, 1, 1, 0, 0, 0)
            },
            new()
            {
                Id = Guid.Parse("7e0a9202-f672-11ed-9ae1-0fc4e648d876"),
                Nome = "SuperAdmin",
                DataCadastro = new DateTime(2023, 1, 1, 0, 0, 0)
            }
        };

        return papeis;
    }
}