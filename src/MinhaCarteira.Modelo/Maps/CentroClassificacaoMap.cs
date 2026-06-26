using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps;

public class CentroClassificacaoMap : BaseMap<CentroClassificacao, Guid>
{
    public override void Configure(EntityTypeBuilder<CentroClassificacao> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataCriacao).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAlteracao).HasColumnType("timestamp without time zone");

        builder.Property(p => p.IdAuxiliar).HasMaxLength(250);
        builder.Property(p => p.Nome).HasMaxLength(250);
    }
}
