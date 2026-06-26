using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoGastos;

public class EvolucaoGastosViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string NomeMesAtual { get; set; }
    public string NomeMesAnterior { get; set; }
    public List<EvolucaoGastosDiarioViewModel> Itens { get; set; } = new();
}
