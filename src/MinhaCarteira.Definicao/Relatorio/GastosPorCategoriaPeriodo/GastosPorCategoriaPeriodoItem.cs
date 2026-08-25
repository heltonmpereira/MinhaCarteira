using System;

namespace MinhaCarteira.Definicao.Relatorio.GastosPorCategoriaPeriodo;

public class GastosPorCategoriaPeriodoItem
{
    public Guid CategoriaId { get; set; }
    public string CategoriaNome { get; set; }
    public string CategoriaPaiNome { get; set; }
    public string CaminhoCompleto => string.IsNullOrEmpty(CategoriaPaiNome)
        ? CategoriaNome
        : $"{CategoriaPaiNome} | {CategoriaNome}";
    public decimal Valor { get; set; }
    public int QuantidadeMovimentos { get; set; }
}
