using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.FluxoCaixa;

public class FluxoCaixaViewModel
{
    public int Ano { get; set; }
    public IList<FluxoCaixaSomatorioViewModel> Somatorios { get; set; }
    public IList<FluxoCaixaCategoriaViewModel> Categorias { get; set; }
}
