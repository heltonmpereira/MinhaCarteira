using System;

namespace MinhaCarteira.AppCliente.ViewModel.Relatorio.GastosPorCategoriaPeriodo;

public class GastosPorCategoriaPeriodoItemViewModel
{
    public Guid CategoriaId { get; set; }
    public string CategoriaNome { get; set; }
    public string CategoriaPaiNome { get; set; }
    public string CaminhoCompleto => string.IsNullOrEmpty(CategoriaPaiNome)
        ? CategoriaNome
        : $"{CategoriaPaiNome} | {CategoriaNome}";
    public decimal Valor { get; set; }
    public int QuantidadeMovimentos { get; set; }
    public decimal Percentual { get; set; }
}
