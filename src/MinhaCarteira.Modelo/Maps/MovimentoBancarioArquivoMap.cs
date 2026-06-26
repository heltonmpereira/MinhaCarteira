using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps
{
    public class MovimentoBancarioArquivoMap : BaseMap<MovimentoBancarioArquivo, Guid>
    {
        public override void Configure(EntityTypeBuilder<MovimentoBancarioArquivo> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.DataCadastro).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");

            builder
                .HasOne(o => o.MovimentoBancario)
                .WithMany(m => m.MovimentoBancarioArquivos);
            //.OnDelete(DeleteBehavior.ClientNoAction);

        }
    }
}
