using System;
using System.Collections.Generic;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.Models.Interface.Resposta;
using Newtonsoft.Json;
using X.PagedList;

namespace MinhaCarteira.AppCliente.ViewModel.Base;

public class ListaBaseViewModel<T> : IListaBaseViewModel
{
    public ListaBaseViewModel()
    {
        FiltroViewModel = new CriterioViewModel();
        OpcaoAtual ??= new FiltroOpcao();

        Itens = new StaticPagedList<T>(
            subset: new List<T>(),
            pageNumber: 1,
            pageSize: 1,
            totalItemCount: 0);
    }
    public ListaBaseViewModel(IList<T> itens, ICriterio filtro, int qtdItens)
        : this()
    {
        var pagina = filtro.Pagina > 0 ? filtro.Pagina : 1;
        Itens = new StaticPagedList<T>(
            subset: itens,
            pageNumber: pagina,
            pageSize: filtro.ItensPorPagina,
            totalItemCount: qtdItens);
    }
    public ListaBaseViewModel(IRespostaPaginadaServico<T> resposta)
    {
        Itens = new StaticPagedList<T>(
            subset: resposta.Dados ?? new List<T>(),
            pageNumber: Math.Max(1, resposta.NumeroPagina),
            pageSize: resposta.ItensPorPagina,
            totalItemCount: resposta.TotalRegistros);
    }

    public ICriterioViewModel FiltroViewModel { get; set; }
    public IPagedList<T> Itens { get; set; }
    public FiltroOpcao OpcaoAtual { get; set; }
    public ICriterio Filtro { get; set; }
    public string FiltroJson { get => JsonConvert.SerializeObject(Filtro); }
}