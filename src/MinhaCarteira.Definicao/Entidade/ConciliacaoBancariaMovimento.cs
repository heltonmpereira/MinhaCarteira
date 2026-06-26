using MinhaCarteira.Definicao.Interface.Entidade;
using System;

namespace MinhaCarteira.Definicao.Entidade;

public class ConciliacaoBancariaMovimento : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public Guid ConciliacaoBancariaId { get; set; }
    public ConciliacaoBancaria ConciliacaoBancaria { get; set; }

    public Guid MovimentoBancarioId { get; set; }
    public MovimentoBancario MovimentoBancario { get; set; }

    public bool Deletado { get; set; }
}