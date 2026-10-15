using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoCreditos;
using System;
using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio;

public class DashboardCreditosRelatorioViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public List<int> Meses { get; set; } = new();
    public Guid? ContaBancariaId { get; set; }
    public List<Guid> ContasBancariasIds { get; set; } = new();
    public IEnumerable<ContaBancariaViewModel> ContasBancarias { get; set; }
    public EvolucaoCreditosViewModel EvolucaoCreditos { get; set; }
    public EvolucaoCreditosMultiMesViewModel EvolucaoCreditosMultiMes { get; set; }
}
