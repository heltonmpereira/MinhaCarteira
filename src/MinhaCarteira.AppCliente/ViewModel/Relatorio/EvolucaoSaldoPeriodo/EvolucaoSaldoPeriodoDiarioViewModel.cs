using System;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldoPeriodo;

public class EvolucaoSaldoPeriodoDiarioViewModel
{
    public DateTime Data { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal MovimentosRealizados { get; set; }
    public decimal MovimentosPlanejados { get; set; }
    public decimal SaldoFinal { get; set; }
}
