using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;
using System;

namespace MinhaCarteira.AppCliente.ViewModel;

public class HomeViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }

    public bool Deletado { get; set; }
}