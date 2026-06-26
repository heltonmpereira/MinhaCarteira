using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldo;

public class EvolucaoSaldoViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public List<EvolucaoSaldoDiarioViewModel> Itens { get; set; } = new();
}
