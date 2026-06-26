using System.ComponentModel;

namespace MinhaCarteira.Definicao.Modelo;

public enum TipoMovimento
{
    [Description("Compra")]
    Debito,
    [Description("Venda")]
    Credito
}
