using System.ComponentModel;

namespace MinhaCarteira.AppCliente.Models;

public enum TipoMovimento
{
    [Description("Compra")]
    Debito,
    [Description("Venda")]
    Credito
}
