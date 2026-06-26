using System.Collections.Generic;

namespace MinhaCarteira.Definicao.Relatorio.FluxoCaixa;

public class FluxoCaixaCategoria
{
    public string Nome { get; set; }
    public IList<FluxoCaixaSomatorio> Somatorios { get; set; }
    public IList<FluxoCaixaCategoria> CategoriasFilhas { get; set; }
}
