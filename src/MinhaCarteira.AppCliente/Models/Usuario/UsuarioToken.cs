using System;
using System.Collections.Generic;
using Dhani.Utilitarios.Helper;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Models.Usuario;

public class UsuarioToken
{
    public UsuarioToken()
    {
        //construtor necessário para a injeção de dependencia
    }

    public UsuarioToken(UsuarioViewModel source)
    {
        this.Mapear(source);
    }

    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Sobrenome { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string NomeCompleto => $"{Nome} {Sobrenome}";
    public bool ManterConectado { get; set; }
    public DateTime DataExpericaoToken { get; set; }

    public string TokenAcesso { get; set; }
    public ICollection<string> Roles { get; set; }
}