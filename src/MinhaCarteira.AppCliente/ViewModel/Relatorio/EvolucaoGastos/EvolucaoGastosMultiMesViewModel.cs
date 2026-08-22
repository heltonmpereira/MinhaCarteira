using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoGastos;

public class EvolucaoGastosMultiMesViewModel
{
    public int Ano { get; set; }
    public List<int> MesesSelecionados { get; set; } = new();
    public List<EvolucaoGastosMesViewModel> Meses { get; set; } = new();
    public List<EvolucaoGastosMultiMesDiarioViewModel> Itens { get; set; } = new();
}

public class EvolucaoGastosMesViewModel
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string Nome { get; set; }
    public string Chave { get; set; }
    public int Ordem { get; set; }
    public bool EhMaisRecente { get; set; }
}

public class EvolucaoGastosMultiMesDiarioViewModel
{
    public int Dia { get; set; }
    public List<EvolucaoGastosValorMesViewModel> ValoresPorMes { get; set; } = new();
}

public class EvolucaoGastosValorMesViewModel
{
    public string ChaveMes { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string NomeMes { get; set; }
    public decimal? GastosAcumulados { get; set; }
    public decimal GastosDiarios { get; set; }
}
