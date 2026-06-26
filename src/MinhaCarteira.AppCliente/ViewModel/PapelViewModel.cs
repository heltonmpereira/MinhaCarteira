using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class PapelViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Observacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
    public List<SelectListItem> UsuariosSelecionados { get; set; }

    private ICollection<UsuarioPapelViewModel> _usuarios;
    public ICollection<UsuarioPapelViewModel> Usuarios
    {
        get => _usuarios;
        set
        {
            _usuarios = value;
            InicializarListaUsuariosSelecionados();
        }
    }

    private void InicializarListaUsuariosSelecionados()
    {
        var items = new List<SelectListItem>();
        Usuarios?
            .ToList()
            .ForEach(f => items.Add(
                new SelectListItem(
                    f.Usuario?.Nome,
                    f.UsuarioId.ToString()
                    )
                )
            );

        UsuariosSelecionados = [.. items];
    }

    public bool Deletado { get; set; }
}