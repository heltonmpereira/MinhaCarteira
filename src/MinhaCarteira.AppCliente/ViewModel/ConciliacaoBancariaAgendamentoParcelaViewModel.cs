using System;

namespace MinhaCarteira.AppCliente.ViewModel;

public class ConciliacaoBancariaAgendamentoParcelaViewModel
{
    public Guid Id { get; set; }
    public Guid ConciliacaoBancariaId { get; set; }
    public ConciliacaoBancariaViewModel ConciliacaoBancaria { get; set; }

    public Guid AgendamentoParcelaId { get; set; }
    public AgendamentoParcelaViewModel AgendamentoParcela { get; set; }
}