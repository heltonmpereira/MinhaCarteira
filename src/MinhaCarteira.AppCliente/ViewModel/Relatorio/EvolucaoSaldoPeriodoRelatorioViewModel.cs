using MinhaCarteira.AppCliente.ViewModel;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldoPeriodo;
using System;
using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio;

public class EvolucaoSaldoPeriodoRelatorioViewModel
{
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    public Guid? ContaBancariaId { get; set; }
    public IEnumerable<ContaBancariaViewModel> ContasBancarias { get; set; }
    public EvolucaoSaldoPeriodoViewModel EvolucaoSaldoPeriodo { get; set; }
}
