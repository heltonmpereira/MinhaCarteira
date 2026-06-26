using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class AuditoriaMap : BaseMap<Auditoria, Guid>
{
    public override void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataHora).HasColumnType("timestamp without time zone");
        builder.Property(p => p.Acao).HasMaxLength(50);
        builder.Property(p => p.Entidade).HasMaxLength(250);
        builder.Property(p => p.EntidadeId).HasMaxLength(250);
        builder.Property(p => p.DadosAntigos).HasColumnType("text");
        builder.Property(p => p.DadosNovos).HasColumnType("text");
        builder.Property(p => p.IpUsuario).HasMaxLength(50);
        builder.Property(p => p.UserAgent).HasMaxLength(1000);
        builder.Property(p => p.Rotina).HasMaxLength(500);

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
