using System;
using System.Collections.Generic;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldoPeriodo;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.GastosPorCategoriaPeriodo;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio;

public class EvolucaoSaldoPeriodoRelatorioViewModel
{
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    public Guid? ContaBancariaId { get; set; }
    public IEnumerable<ContaBancariaViewModel> ContasBancarias { get; set; }
    public EvolucaoSaldoPeriodoViewModel EvolucaoSaldoPeriodo { get; set; }
    public GastosPorCategoriaPeriodoViewModel GastosPorCategoriaPeriodo { get; set; }
}
