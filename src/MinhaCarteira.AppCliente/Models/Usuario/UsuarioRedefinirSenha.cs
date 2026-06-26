using System;

namespace MinhaCarteira.AppCliente.Models.Usuario;

public class UsuarioRedefinirSenha
{
    public Guid Id { get; set; }
    public string CodigoRedefinicaoSenha { get; set; }
    public string Password { get; set; }
}