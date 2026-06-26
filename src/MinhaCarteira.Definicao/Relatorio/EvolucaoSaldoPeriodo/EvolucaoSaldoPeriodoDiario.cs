using System;

namespace MinhaCarteira.Definicao.Relatorio.EvolucaoSaldoPeriodo;

public class EvolucaoSaldoPeriodoDiario
{
    public DateTime Data { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal MovimentosRealizados { get; set; }
    public decimal MovimentosPlanejados { get; set; }
    public decimal SaldoFinal { get; set; }
}
