using System;
using System.ComponentModel;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class ImportarArquivoViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }

    [DisplayName("Conta Bancária")]
    public Guid ContaBancariaId { get; set; }
    public ContaBancariaViewModel ContaBancaria { get; set; }
    public string NomeContaBancaria => ContaBancaria?.Nome;

    public bool Deletado { get; set; }
}
