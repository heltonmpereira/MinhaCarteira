using System.Collections.Generic;

namespace MinhaCarteira.Definicao.Relatorio.EvolucaoCreditos;

public class EvolucaoCreditos
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string NomeMesAtual { get; set; }
    public string NomeMesAnterior { get; set; }
    public List<EvolucaoCreditosDiario> Itens { get; set; } = new();
}
