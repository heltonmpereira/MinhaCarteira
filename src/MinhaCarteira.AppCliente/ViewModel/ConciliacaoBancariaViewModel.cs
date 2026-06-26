using System;
using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel;

public class ConciliacaoBancariaViewModel
{
    public Guid Id { get; set; }
    public DateTime DataCadastro { get; set; }

    public Guid ProprietarioId { get; set; }
    public UsuarioViewModel Proprietario { get; set; }

    public ICollection<ConciliacaoBancariaMovimentoViewModel> Movimentos { get; set; }
    public ICollection<ConciliacaoBancariaAgendamentoParcelaViewModel> AgendamentoParcelas { get; set; }
}