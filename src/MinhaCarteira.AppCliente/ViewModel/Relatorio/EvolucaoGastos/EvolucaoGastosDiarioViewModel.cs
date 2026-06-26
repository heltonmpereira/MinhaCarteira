using System;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoGastos;

public class EvolucaoGastosDiarioViewModel
{
    public DateTime Data { get; set; }
    public int Dia { get; set; }
    public decimal GastosMesAtual { get; set; }
    public decimal GastosMesAnterior { get; set; }
    public decimal? GastosAcumuladosMesAtual { get; set; }
    public decimal GastosAcumuladosMesAnterior { get; set; }
}
