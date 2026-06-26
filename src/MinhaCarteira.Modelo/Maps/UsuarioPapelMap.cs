using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class UsuarioPapelMap : BaseMap<UsuarioPapel, Guid>
{
    public override void Configure(EntityTypeBuilder<UsuarioPapel> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.DataCadastro).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");

        builder
            .HasOne(o => o.Usuario)
            .WithMany(m => m.Papeis);
        //.OnDelete(DeleteBehavior.ClientNoAction);

        builder
            .HasOne(o => o.Papel)
            .WithMany(m => m.Usuarios);
        //.OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasData(ObterDadosIniciais());
    }

    private static UsuarioPapel[] ObterDadosIniciais()
    {
        var usuarioPapeis = new UsuarioPapel[] {
        new UsuarioPapel{
            Id = Guid.Parse("7e0a9203-f672-11ed-9ae1-0fc4e648d876"),
            UsuarioId = Guid.Parse("7e0a9200-f672-11ed-9ae1-0fc4e648d876"),
            PapelId = Guid.Parse("7e0a9201-f672-11ed-9ae1-0fc4e648d876")
        },
        new UsuarioPapel{
            Id = Guid.Parse("7e0a9204-f672-11ed-9ae1-0fc4e648d876"),
            UsuarioId = Guid.Parse("7e0a9200-f672-11ed-9ae1-0fc4e648d876"),
            PapelId = Guid.Parse("7e0a9202-f672-11ed-9ae1-0fc4e648d876")
        }

    };

        return usuarioPapeis;
    }
}