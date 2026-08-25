using System;
using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.GastosPorCategoriaPeriodo;

public class GastosPorCategoriaPeriodoViewModel
{
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    public decimal TotalGastos { get; set; }
    public List<GastosPorCategoriaPeriodoItemViewModel> Itens { get; set; } = new();
}
