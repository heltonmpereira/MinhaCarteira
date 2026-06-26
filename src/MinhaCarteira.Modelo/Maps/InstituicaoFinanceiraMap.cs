using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Modelo.Maps.Base;
using System;

namespace MinhaCarteira.Modelo.Maps;

public class InstituicaoFinanceiraMap : BaseMap<InstituicaoFinanceira, Guid>
{
    public override void Configure(EntityTypeBuilder<InstituicaoFinanceira> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.DataCriacao).HasColumnType("timestamp without time zone");
        builder.Property(p => p.DataAlteracao).HasColumnType("timestamp without time zone");

        builder.Property(p => p.Nome).HasMaxLength(250);
        builder.Property(p => p.CorFundo).HasMaxLength(150);
        //builder.Property(p => p.NomeIcone).HasMaxLength(250);
        builder.Property(p => p.DelimitadorCsv).HasMaxLength(1);
        builder.Property(p => p.MapeamentoCamposCsv).HasMaxLength(2500);

        builder
            .HasOne(o => o.Icone)
            .WithMany()
            .HasForeignKey(a => a.IconeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(ObterDadosIniciais());
    }

    private static InstituicaoFinanceira[] ObterDadosIniciais()
    {
        var instituicoes = new InstituicaoFinanceira[]
        {
            new()
            {
                Id = Guid.Parse("7e0a9205-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "Itaú",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd201-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9206-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "Bradesco",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd202-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9207-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "Santander",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd203-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9208-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "Caixa",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd204-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9209-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "Banco do Brasil",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd205-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9210-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "PicPay",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd206-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9211-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "NuBank",
                DelimitadorCsv = ",",
                IconeId = Guid.Parse("5cafd207-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9212-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "C6",
                NomeCamposPrimeiraLinha = true,
                MapeamentoCamposCsv = "DataMovimento={% row.Data_de_Compra %};Valor={% row.Valor_em_R | regex.replace  \"[.]\" \",\" %};Competencia={% date.parse row.Data_de_Compra '%d/%m/%Y' | date.to_string '%Y%m'  %};Descricao={% row.Descrição %}  {% case row.Parcela %} {% when \"Única\" %}{% else %} ({% row.Parcela %}) {% endcase %}",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd208-9535-4a01-90dc-1ef77a7a7cd0")
            },
            new()
            {
                Id = Guid.Parse("7e0a9213-f672-11ed-9ae1-0fc4e648d876"),
                DataCriacao = new DateTime(2023, 1, 1, 0, 0, 0),
                Nome = "Cora",
                DelimitadorCsv = ";",
                IconeId = Guid.Parse("5cafd209-9535-4a01-90dc-1ef77a7a7cd0")
            }
        };


        return instituicoes;
    }
}
