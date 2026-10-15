using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoCreditos;

public class EvolucaoCreditosMultiMesViewModel
{
    public int Ano { get; set; }
    public List<int> MesesSelecionados { get; set; } = new();
    public List<EvolucaoCreditosMesViewModel> Meses { get; set; } = new();
    public List<EvolucaoCreditosMultiMesDiarioViewModel> Itens { get; set; } = new();
}

public class EvolucaoCreditosMesViewModel
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string Nome { get; set; }
    public string Chave { get; set; }
    public int Ordem { get; set; }
    public bool EhMaisRecente { get; set; }
}

public class EvolucaoCreditosMultiMesDiarioViewModel
{
    public int Dia { get; set; }
    public List<EvolucaoCreditosValorMesViewModel> ValoresPorMes { get; set; } = new();
}

public class EvolucaoCreditosValorMesViewModel
{
    public string ChaveMes { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public string NomeMes { get; set; }
    public decimal? CreditosAcumulados { get; set; }
    public decimal CreditosDiarios { get; set; }
}
