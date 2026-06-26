using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps;

public class AgendamentoMap : BaseMap<Agendamento, Guid>
{
    public override void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataInicial).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataCriacao).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAlteracao).HasColumnType("timestamp without time zone");

        builder.Property(p => p.IdAuxiliar).HasMaxLength(200);
        builder.Property(p => p.Descricao).HasMaxLength(200);
        builder.Property(p => p.Valor).HasPrecision(18, 6);
        builder.Property(p => p.Tipo).HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TipoMovimento>(v));
        builder.Property(p => p.TipoParcelas).HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TipoParcelas>(v));
        builder.Property(p => p.TipoRecorrencia).HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TipoRecorrencia>(v));

        builder
            .HasMany(m => m.Parcelas)
            .WithOne(o => o.Agendamento)
            .OnDelete(DeleteBehavior.Cascade);

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
