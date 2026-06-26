using MinhaCarteira.Definicao.Interface.Entidade;
using System;

namespace MinhaCarteira.Definicao.Entidade;

public class ConciliacaoBancariaAgendamentoParcela : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public Guid ConciliacaoBancariaId { get; set; }
    public ConciliacaoBancaria ConciliacaoBancaria { get; set; }

    public Guid AgendamentoParcelaId { get; set; }
    public AgendamentoParcela AgendamentoParcela { get; set; }

    public bool Deletado { get; set; }
}