using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps
{
    public class ArquivoMap : BaseMap<Arquivo, Guid>
    {
        public override void Configure(EntityTypeBuilder<Arquivo> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.SubPasta).HasMaxLength(250);
            builder.Property(p => p.Nome).HasMaxLength(250);
            builder.Property(p => p.Extensao).HasMaxLength(50);
            builder.Property(p => p.DataCadastro).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");
        }
    }
}
