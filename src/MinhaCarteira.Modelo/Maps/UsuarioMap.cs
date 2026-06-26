using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;

namespace MinhaCarteira.Modelo.Maps;

public class UsuarioMap : BaseMap<Usuario, Guid>
{
    public override void Configure(EntityTypeBuilder<Usuario> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Nome).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Sobrenome).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Username).HasMaxLength(50).IsRequired();
        builder.Property(p => p.PasswordHash).HasMaxLength(200).IsRequired();
        builder.Property(p => p.DataCadastro).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAtualizacao).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");

        builder.Property(p => p.CodigoRedefinicaoSenha).HasMaxLength(200);

        builder.HasIndex(i => i.Email).IsUnique();
        builder.HasIndex(i => i.Username).IsUnique();

        builder
            .HasOne(o => o.Organizacao)
            .WithOne(o => o.Administrador)
            .HasForeignKey<Usuario>(o => o.OrganizacaoId);

        builder.HasData(ObterDadosIniciais());
    }

    private static Usuario[] ObterDadosIniciais()
    {
        var usuarios = new Usuario[] {
            new Usuario{
                Id = Guid.Parse("7e0a9200-f672-11ed-9ae1-0fc4e648d876"),
                Nome = "Administrador",
                Sobrenome = "do Sistema",
                Username = "admin",
                Email = "admin@admin.com",
                PasswordHash = "AITMdMEsqiixw35g6qbq+zbaYM2HttO8uXFdJSg96xVnUn2AatqTCDcKqSVPlbzulA==",
                DataCadastro = new DateTime(2023, 1, 1, 0, 0, 0),
                OrganizacaoId = Guid.Parse("7e0a9204-f672-11ed-9ae1-0fc4e648d876")
            }
        };

        return usuarios;
    }
}