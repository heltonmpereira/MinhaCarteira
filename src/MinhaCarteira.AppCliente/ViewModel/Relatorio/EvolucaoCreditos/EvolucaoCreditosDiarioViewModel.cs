using System;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoCreditos;

public class EvolucaoCreditosDiarioViewModel
{
    public DateTime Data { get; set; }
    public int Dia { get; set; }
    public decimal CreditosMesAtual { get; set; }
    public decimal CreditosMesAnterior { get; set; }
    public decimal? CreditosAcumuladosMesAtual { get; set; }
    public decimal CreditosAcumuladosMesAnterior { get; set; }
}
