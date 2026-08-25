using System;
using System.Collections.Generic;

namespace MinhaCarteira.Definicao.Relatorio.GastosPorCategoriaPeriodo;

public class GastosPorCategoriaPeriodo
{
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    public decimal TotalGastos { get; set; }
    public List<GastosPorCategoriaPeriodoItem> Itens { get; set; } = new();
}
