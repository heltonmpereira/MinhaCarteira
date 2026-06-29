using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class LogMap : BaseMap<Log, Guid>
{
    public override void Configure(EntityTypeBuilder<Log> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataHora).HasColumnType("timestamp with time zone");
        builder.Property(p => p.Categoria).HasMaxLength(500);
        builder.Property(p => p.Mensagem).HasColumnType("text");
        builder.Property(p => p.DadosSerializados).HasColumnType("text");
        builder.Property(p => p.StackTrace).HasColumnType("text");
        builder.Property(p => p.IpUsuario).HasMaxLength(50);
        builder.Property(p => p.UserAgent).HasMaxLength(1000);
        builder.Property(p => p.Url).HasMaxLength(1000);
        builder.Property(p => p.MetodoHttp).HasMaxLength(10);

        builder
            .HasOne(o => o.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(o => o.Organizacao)
            .WithMany()
            .HasForeignKey(a => a.OrganizacaoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
