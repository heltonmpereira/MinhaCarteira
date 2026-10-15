using System;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class HomeViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }

    public bool Deletado { get; set; }
}