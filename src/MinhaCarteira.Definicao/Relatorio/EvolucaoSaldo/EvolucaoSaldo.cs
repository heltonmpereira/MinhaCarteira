using System;
using System.Collections.Generic;

namespace MinhaCarteira.Definicao.Relatorio.EvolucaoSaldo;

public class EvolucaoSaldo
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public List<EvolucaoSaldoDiario> Itens { get; set; } = new();
}
