using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class AgendamentoParcelaViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public Guid AgendamentoId { get; set; }
    public AgendamentoViewModel Agendamento { get; set; }

    [DisplayName("Data")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
    public DateTime Data { get; set; } = DateTime.Now.Date;
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal Valor { get; set; }
    //public bool EstahPaga { get; set; }
    //public bool EstahConciliada { get; set; }
    [DisplayName("Alterar apenas esta parcela?")]
    public bool AlterarAPenasEstaParcela { get; set; } = false;
    [DisplayName("Observação")]
    public string Observacao { get; set; }
    [DisplayName("Parcela")]
    public int NumeroParcela { get; set; }
    [DisplayName("Despesa opcional ou parcela não realizada este mês")]
    public bool DespesaOpcional { get; set; }


    [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
    public DateTime? DataPagamento { get; set; }
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal? ValorPago { get; set; }
    public decimal? ValorRealPago => Agendamento?.Tipo == TipoMovimento.Credito ? ValorPago : (ValorPago ?? 0) * -1;

    public Guid? PessoaId { get; set; }
    public PessoaViewModel Pessoa { get; set; }
    public string NomePessoa => Pessoa != null
        ? Pessoa.Nome
        : Agendamento?.NomePessoa ?? string.Empty;

    public Guid? ContaBancariaId { get; set; }
    public ContaBancariaViewModel ContaBancaria { get; set; }
    public string NomeContaBancaria => ContaBancaria != null
        ? ContaBancaria.Nome
        : string.Empty;

    public Guid? ConciliacaoBancariaAgendamentoParcelaId { get; set; }
    public ConciliacaoBancariaAgendamentoParcelaViewModel ConciliacaoBancariaAgendamentoParcela { get; set; }

    public StatusParcela StatusParcela
    {
        get
        {
            var mesAnoConta = int.Parse($"{Data.Year}{Data.Month:d2}");
            var mesAnoAtual = int.Parse($"{DateTime.Now.Year}{DateTime.Now.Month:d2}");

            if (ConciliacaoBancariaAgendamentoParcelaId != null && ConciliacaoBancariaAgendamentoParcelaId != Guid.Empty)
                return StatusParcela.Conciliada;
            else if (ValorPago != null)
                return StatusParcela.Paga;
            else if (mesAnoAtual > mesAnoConta)
                return StatusParcela.Vencida;
            else return StatusParcela.Aberta;
        }
    }

    public bool Deletado { get; set; }
}