using System;
using System.Collections.Generic;

namespace MinhaCarteira.Definicao.Relatorio.EvolucaoSaldoPeriodo;

public class EvolucaoSaldoPeriodo
{
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    public decimal SaldoInicialTotal { get; set; }
    public List<EvolucaoSaldoPeriodoDiario> Itens { get; set; } = new();
}
