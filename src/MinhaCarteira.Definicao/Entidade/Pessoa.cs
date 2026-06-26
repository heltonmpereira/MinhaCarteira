using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class Pessoa : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public string Nome { get; set; }
    public bool EhCliente { get; set; }
    public bool EhFornecedor { get; set; }
    public string NomeExtrato { get; set; }
    public bool IgnorarMovimentacoes { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }

    public Guid? CentroClassificacaoId { get; set; }
    public CentroClassificacao CentroClassificacao { get; set; }

    public Guid? CategoriaId { get; set; }
    public Categoria Categoria { get; set; }

    public bool Deletado { get; set; }
}