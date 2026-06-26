using System;
using System.ComponentModel.DataAnnotations.Schema;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class AgendamentoParcela : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public Guid AgendamentoId { get; set; }
    public Agendamento Agendamento { get; set; }
    public DateTime Data { get; set; }
    public DateTime? DataPagamento { get; set; }
    public decimal Valor { get; set; }
    public decimal? ValorPago { get; set; }
    public int NumeroParcela { get; set; }
    public string Observacao { get; set; }
    public bool EstahPaga { get; set; }
    public bool EstahConciliada { get; set; }
    public bool Deletado { get; set; }
    public bool DespesaOpcional { get; set; }

    [NotMapped]
    public bool AlterarAPenasEstaParcela { get; set; } = false;

    public Guid? PessoaId { get; set; }
    public Pessoa Pessoa { get; set; }

    public Guid? ContaBancariaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; }

    //public ICollection<MovimentoBancario> Movimentos { get; set; }
    public Guid? ConciliacaoBancariaAgendamentoParcelaId { get; set; }
    public ConciliacaoBancariaAgendamentoParcela ConciliacaoBancariaAgendamentoParcela { get; set; }
}
