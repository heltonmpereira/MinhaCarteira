using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class ContaBancariaViewModel : BaseViewModel, IEntidade<Guid>
{
    public ContaBancariaViewModel()
    {
        DataSaldoInicial = DateTime.Today;
    }

    public Guid Id { get; set; }
    public int Ordem { get; set; }
    public string Nome { get; set; }
    public string Agencia { get; set; }
    public string Conta { get; set; }
    [DataType(DataType.Date)]
    [Display(Name = "Data incial do saldo")]
    public DateTime DataSaldoInicial { get; set; }
    [DataType(DataType.Currency)]
    [Display(Name = "Valor inicial")]
    public decimal ValorSaldoInicial { get; set; }
    [Display(Name = "Saldo")]
    [DataType(DataType.Currency)]
    public decimal ValorSaldoAtual { get; set; }
    [Display(Name = "Exibir o saldo atual na tela inicial?")]
    public bool ExibirNaTelaInicial { get; set; }

    [DisplayName("Cadastro")]
    public DateTime DataCriacao { get; set; }
    [DisplayName("Alteração")]
    public DateTime? DataAlteracao { get; set; }

    [Display(Name = "Instituição Financeira")]
    public Guid InstituicaoFinanceiraId { get; set; }
    public InstituicaoFinanceiraViewModel InstituicaoFinanceira { get; set; }
    public string InstituicaoFinanceiraNome => InstituicaoFinanceira?.Nome;

    public Guid ProprietarioId { get; set; }
    public UsuarioViewModel Proprietario { get; set; }

    public bool Deletado { get; set; }
}
