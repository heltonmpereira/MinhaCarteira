using System.Collections.Generic;
using System.Linq;

namespace MinhaCarteira.Definicao.Relatorio.FluxoCaixa;

public class FluxoCaixa
{
    private static List<FluxoCaixaSomatorio> CarregarSomatorios(IList<FluxoCaixaItem> itens)
    {
        var retorno = new List<FluxoCaixaSomatorio>
        {
            new()
            {
                Titulo = "Janeiro",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Janeiro,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Janeiro
            },
            new()
            {
                Titulo = "Fevereiro",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Fevereiro,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Fevereiro
            },
            new()
            {
                Titulo = "Março",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Marco,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Marco
            },
            new()
            {
                Titulo = "Abril",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Abril,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Abril
            },
            new()
            {
                Titulo = "Maio",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Maio,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Maio
            },
            new()
            {
                Titulo = "Junho",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Junho,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Junho
            },
            new()
            {
                Titulo = "Julho",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Julho,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Julho
            },
            new()
            {
                Titulo = "Agosto",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Agosto,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Agosto
            },
            new()
            {
                Titulo = "Setembro",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Setembro,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Setembro
            },
            new()
            {
                Titulo = "Outubro",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Outubro,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Outubro
            },
            new()
            {
                Titulo = "Novembro",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Novembro,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Novembro
            },
            new()
            {
                Titulo = "Dezembro",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Dezembro,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Dezembro
            },
            new()
            {
                Titulo = "Total",
                ValorPrevisto = itens.FirstOrDefault(w => w.Origem == "Planejado")?.Total,
                ValorRealizado = itens.FirstOrDefault(w => w.Origem == "Realizado")?.Total
            }

        };

        return retorno;
    }
    private static List<FluxoCaixaSomatorio> CalcularSomaCategorias(IList<FluxoCaixaCategoria> categorias)
    {
        var retorno = new List<FluxoCaixaSomatorio>();

        foreach (var categoria in categorias)
        {
            var somas = categoria.Somatorios
                .GroupBy(g => g.Titulo)
                .Select(s => new FluxoCaixaSomatorio()
                {
                    Titulo = s.Key,
                    ValorPrevisto = s.Sum(s => s.ValorPrevisto),
                    ValorRealizado = s.Sum(s => s.ValorRealizado)
                })
                .ToList();

            retorno.AddRange(somas);
        }

        var somatorio = retorno
            .GroupBy(g => g.Titulo)
            .Select(s => new FluxoCaixaSomatorio()
            {
                Titulo = s.Key,
                ValorPrevisto = s.Sum(s => s.ValorPrevisto),
                ValorRealizado = s.Sum(s => s.ValorRealizado)
            })
            .ToList();

        return somatorio;
    }

    public FluxoCaixa(int ano, IList<FluxoCaixaItem> itens)
    {
        Ano = ano;
        Categorias = itens
            .GroupBy(g => g.CategoriaPaiNome)
            .Select(s => new FluxoCaixaCategoria() { Nome = s.Key })
            .ToList();

        foreach (var ctgPai in Categorias)
        {
            ctgPai.CategoriasFilhas = itens
                .Where(w => w.CategoriaPaiNome == ctgPai.Nome)
                .GroupBy(g => g.CategoriaNome)
                .Select(s => new FluxoCaixaCategoria() { Nome = s.Key })
                .ToList();

            foreach (var ctgFilha in ctgPai.CategoriasFilhas)
            {
                var registros = itens
                    .Where(w => w.CategoriaPaiNome == ctgPai.Nome && w.CategoriaNome == ctgFilha.Nome)
                    .ToList();

                ctgFilha.Somatorios = CarregarSomatorios(registros);
            }

            ctgPai.Somatorios = CalcularSomaCategorias(ctgPai.CategoriasFilhas);
        }

        Somatorios = CalcularSomaCategorias(Categorias);
    }



    public int Ano { get; set; }
    public IList<FluxoCaixaSomatorio> Somatorios { get; set; }
    public IList<FluxoCaixaCategoria> Categorias { get; set; }
}
