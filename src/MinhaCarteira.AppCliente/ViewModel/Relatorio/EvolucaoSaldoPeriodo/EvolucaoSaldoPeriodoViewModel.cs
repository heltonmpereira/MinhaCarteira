using System;
using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldoPeriodo;

public class EvolucaoSaldoPeriodoViewModel
{
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    public decimal SaldoInicialTotal { get; set; }
    public List<EvolucaoSaldoPeriodoDiarioViewModel> Itens { get; set; } = new();
}
