using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class InstituicaoFinanceira : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    //public string Icone { get; set; }
    public string CorFundo { get; set; }
    //public string NomeArquivo { get; set; }
    public bool NomeCamposPrimeiraLinha { get; set; }
    public string MapeamentoCamposCsv { get; set; }
    public string DelimitadorCsv { get; set; }

    public DateTime? DataAlteracao { get; set; }
    public DateTime DataCriacao { get; set; }

    public bool Deletado { get; set; }

    public Guid? IconeId { get; set; }
    public Icone Icone { get; set; }
}