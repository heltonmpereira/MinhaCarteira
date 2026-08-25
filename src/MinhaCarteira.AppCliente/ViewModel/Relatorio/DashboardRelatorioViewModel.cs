using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoGastos;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldo;
using System;
using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio;

public class DashboardRelatorioViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public List<int> Meses { get; set; } = new();
    public Guid? ContaBancariaId { get; set; }
    public IEnumerable<ContaBancariaViewModel> ContasBancarias { get; set; }
    public EvolucaoSaldoViewModel EvolucaoSaldo { get; set; }
    public EvolucaoGastosViewModel EvolucaoGastos { get; set; }
    public EvolucaoGastosMultiMesViewModel EvolucaoGastosMultiMes { get; set; }
}

