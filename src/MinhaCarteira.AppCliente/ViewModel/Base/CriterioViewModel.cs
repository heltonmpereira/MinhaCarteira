using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinhaCarteira.AppCliente.Models.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinhaCarteira.AppCliente.ViewModel.Base;

public class CriterioViewModel : ICriterioViewModel
{
    private List<GrupoFiltro> TratarGrupos(string json)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var grupos = JsonConvert.DeserializeObject<List<GrupoFiltro>>(json, settings);
            grupos = grupos
                .Where(w => w.PersistirGrupo)
                .GroupBy(g => g.NomeGrupoKey)
                .Select(s => s.FirstOrDefault())
                .ToList();

            grupos
                .ForEach(grupo => grupo.Filtros = grupo.Filtros.Where(w => w != null)
                    .GroupBy(g => new { g.NomePropriedade, g.Operador, g.Valor })
                    .Select(s => s.FirstOrDefault())
                    .ToList());

            grupos
                .ForEach(grupo => grupo.Filtros
                    .ForEach(filtro =>
                        filtro.Valor = string.IsNullOrWhiteSpace(Convert.ToString(filtro.Valor))
                            ? null
                            : filtro.Valor));

            grupos.ForEach(f => f.Filtros.RemoveAll(w => w == null));
            grupos.RemoveAll(w => w.Filtros == null || !w.Filtros.Any());

            return grupos;
        }
        catch
        {
            return null;
        }
    }

    public CriterioViewModel()
    {
    }
    public CriterioViewModel(List<SelectListItem> colunas, List<GrupoFiltro> grupoFiltro) : this()
    {
        Colunas = colunas;
        GruposFiltro = grupoFiltro;
        OrdenarFiltros();
    }
    public void OrdenarFiltros()
    {
        var idx = 1;
        foreach (var grupo in GruposFiltro)
            foreach (var filtro in grupo.Filtros)
            {
                filtro.Id = idx;
                idx++;
            }
    }

    public int PaginaAtual { get; set; }
    public int ItensPorPagina { get; set; }
    public string Ordenacao { get; set; }
    public FiltroOpcao FiltroAtual { get; set; }
    public List<GrupoFiltro> GruposFiltro { get; set; } = [];
    public List<SelectListItem> Colunas { get; set; }

    [JsonIgnore]
    public string FiltroJson
    {
        get => JsonConvert.SerializeObject(GruposFiltro);
        set
        {
            GruposFiltro = TratarGrupos(value) ?? GruposFiltro;
        }
    }
    public string MetaDados { get; set; }
}