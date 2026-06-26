using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps;

public class RegraImportacaoMap : BaseMap<RegraImportacao, Guid>
{
    public override void Configure(EntityTypeBuilder<RegraImportacao> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.DataCriacao).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAlteracao).HasColumnType("timestamp without time zone");

        builder.Property(p => p.Descricao).HasMaxLength(200);
        builder.Property(p => p.ValorMinimo).HasPrecision(18, 6);
        builder.Property(p => p.ValorMaximo).HasPrecision(18, 6);

        builder
            .HasOne(o => o.Categoria)
            .WithMany()
            .HasForeignKey(f => f.CategoriaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(o => o.CentroClassificacao)
            .WithMany()
            .HasForeignKey(f => f.CentroClassificacaoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(o => o.Pessoa)
            .WithMany()
            .HasForeignKey(f => f.PessoaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(o => o.ContaBancaria)
            .WithMany()
            .HasForeignKey(f => f.ContaBancariaId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
