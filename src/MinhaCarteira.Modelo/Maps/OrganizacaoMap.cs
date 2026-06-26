using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps;

public class OrganizacaoMap : BaseMap<Organizacao, Guid>
{
    public override void Configure(EntityTypeBuilder<Organizacao> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.DataCadastro).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAtualizacao).HasColumnType("timestamp without time zone");

        builder.Property(p => p.Nome).HasMaxLength(150).IsRequired();
        //builder.Property(p => p.AdministradorId).HasColumnName("UsuarioId");

        builder
            .HasOne(o => o.Administrador)
            .WithOne(o => o.Organizacao)
            .HasForeignKey<Usuario>(f => f.OrganizacaoId);

        builder.HasData(ObterDadosIniciais());
    }

    private static Organizacao[] ObterDadosIniciais()
    {
        var organizacoes = new Organizacao[] {
            new Organizacao {
                Id = Guid.Parse("7e0a9204-f672-11ed-9ae1-0fc4e648d876"),
                Nome = "Sistema Minha Carteira",
                DataCadastro = new DateTime(2023, 1, 1, 0, 0, 0),
                AdministradorId = Guid.Parse("7e0a9200-f672-11ed-9ae1-0fc4e648d876")
            }
        };

        return organizacoes;
    }
}
