using System.Collections.Generic;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;
using X.PagedList;

namespace MinhaCarteira.AppCliente.ViewModel;

public class ListaMovimentoBancarioViewModel : BaseViewModel, IListaBaseViewModel
{
    public ContaBancariaViewModel ContaSelecionada { get; set; }

    public ICriterioViewModel FiltroViewModel { get; set; }
    public IList<ContaBancariaViewModel> Contas { get; set; }
    public IPagedList<MovimentoBancarioViewModel> Itens { get; set; }
}