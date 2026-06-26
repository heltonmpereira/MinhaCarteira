using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.FluxoCaixa;

public class FluxoCaixaCategoriaViewModel
{
    public string Nome { get; set; }
    public IList<FluxoCaixaSomatorioViewModel> Somatorios { get; set; }
    public IList<FluxoCaixaCategoriaViewModel> CategoriasFilhas { get; set; }
}
