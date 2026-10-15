using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoCreditos;

public class EvolucaoCreditosViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string NomeMesAtual { get; set; }
    public string NomeMesAnterior { get; set; }
    public List<EvolucaoCreditosDiarioViewModel> Itens { get; set; } = new();
}
