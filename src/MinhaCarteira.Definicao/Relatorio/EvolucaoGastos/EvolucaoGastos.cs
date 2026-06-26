using System;
using System.Collections.Generic;

namespace MinhaCarteira.Definicao.Relatorio.EvolucaoGastos;

public class EvolucaoGastos
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string NomeMesAtual { get; set; }
    public string NomeMesAnterior { get; set; }
    public List<EvolucaoGastosDiario> Itens { get; set; } = new();
}
