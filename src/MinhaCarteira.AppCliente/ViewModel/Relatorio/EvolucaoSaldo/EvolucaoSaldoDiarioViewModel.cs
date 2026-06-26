using System;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldo;

public class EvolucaoSaldoDiarioViewModel
{
    public DateTime Data { get; set; }
    public decimal SaldoPlanejado { get; set; }
    public decimal SaldoRealizado { get; set; }
    public decimal SaldoAcumuladoPlanejado { get; set; }
    public decimal SaldoAcumuladoRealizado { get; set; }
}
