using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class RegraImportacao : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public string PalavrasChave { get; set; }
    public decimal? ValorMinimo { get; set; }
    public decimal? ValorMaximo { get; set; }
    public string Descricao { get; set; }

    public Guid? ContaBancariaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; }

    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; }

    public Guid CentroClassificacaoId { get; set; }
    public CentroClassificacao CentroClassificacao { get; set; }

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }

    public bool Deletado { get; set; }
}