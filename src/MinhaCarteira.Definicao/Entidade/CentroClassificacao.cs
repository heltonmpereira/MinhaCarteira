using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class CentroClassificacao : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public string Nome { get; set; }
    public bool EhReceita { get; set; }
    public bool EhDespesa { get; set; }
    public bool IgnorarMovimentacoes { get; set; }
    public bool Deletado { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public Guid ProprietarioId { get; set; }
}