using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.Models.Interface;

public interface ICriterioViewModel
{
    int PaginaAtual { get; set; }
    public int ItensPorPagina { get; set; }
    public string Ordenacao { get; set; }
    FiltroOpcao FiltroAtual { get; set; }
    List<GrupoFiltro> GruposFiltro { get; set; }
    List<SelectListItem> Colunas { get; set; }

    string FiltroJson { get; }
    string MetaDados { get; set; }
}