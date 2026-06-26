using System;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class UsuarioPapelViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public Guid PapelId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCadastro { get; set; }

    public PapelViewModel Papel { get; set; }
    public UsuarioViewModel Usuario { get; set; }

    public bool Deletado { get; set; }
}