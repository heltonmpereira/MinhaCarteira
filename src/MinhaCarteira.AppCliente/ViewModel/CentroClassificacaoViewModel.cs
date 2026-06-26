using System;
using System.ComponentModel;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class CentroClassificacaoViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public string Nome { get; set; }
    [DisplayName("Receita?")]
    public bool EhReceita { get; set; }
    [DisplayName("Despesa?")]
    public bool EhDespesa { get; set; }
    [DisplayName("Ignorar movimentos bancários?")]
    public bool IgnorarMovimentacoes { get; set; }

    [DisplayName("Cadastro")]
    public DateTime DataCriacao { get; set; }
    [DisplayName("Alteração")]
    public DateTime? DataAlteracao { get; set; }

    public Guid ProprietarioId { get; set; }

    public bool Deletado { get; set; }
}
