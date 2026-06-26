using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class ConciliacaoBancaria : IEntidade<Guid>
{
    public ConciliacaoBancaria() { }
    public ConciliacaoBancaria(ICollection<ConciliacaoBancariaMovimento> movimentos, ICollection<ConciliacaoBancariaAgendamentoParcela> agendamentoParcelas)
    {
        Movimentos = movimentos;
        AgendamentoParcelas = agendamentoParcelas;
    }

    public Guid Id { get; set; }
    public DateTime DataCadastro { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }

    public bool Deletado { get; set; }

    [Required]
    public ICollection<ConciliacaoBancariaMovimento> Movimentos { get; set; }
    [Required]
    public ICollection<ConciliacaoBancariaAgendamentoParcela> AgendamentoParcelas { get; set; }
}