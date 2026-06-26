using System;

namespace MinhaCarteira.AppCliente.ViewModel;

public class ConciliacaoBancariaMovimentoViewModel
{
    public Guid Id { get; set; }
    public Guid ConciliacaoBancariaId { get; set; }
    public ConciliacaoBancariaViewModel ConciliacaoBancaria { get; set; }

    public Guid MovimentoBancarioId { get; set; }
    public MovimentoBancarioViewModel MovimentoBancario { get; set; }
}