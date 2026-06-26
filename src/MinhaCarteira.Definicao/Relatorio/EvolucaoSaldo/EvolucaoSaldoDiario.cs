using System;
using System.Collections.Generic;

namespace MinhaCarteira.Definicao.Relatorio.EvolucaoSaldo;

public class EvolucaoSaldoDiario
{
    public DateTime Data { get; set; }
    public decimal SaldoPlanejado { get; set; }
    public decimal SaldoRealizado { get; set; }
    public decimal SaldoAcumuladoPlanejado { get; set; }
    public decimal SaldoAcumuladoRealizado { get; set; }
}
