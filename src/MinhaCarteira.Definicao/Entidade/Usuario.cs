using System;
using System.Collections.Generic;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class Usuario : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Sobrenome { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }

    public string CodigoRedefinicaoSenha { get; set; }

    public Guid OrganizacaoId { get; set; }
    public Organizacao Organizacao { get; set; }

    public ICollection<UsuarioPapel> Papeis { get; set; }

    public bool Deletado { get; set; }
}