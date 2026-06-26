using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class UsuarioViewModel : BaseViewModel, IEntidade<Guid>
{
    public UsuarioViewModel()
    {
        PasswordHash = "123456";
    }

    public Guid Id { get; set; }
    public Guid OrganizacaoId { get; set; }

    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    public string Sobrenome { get; set; }

    [DisplayName("E-mail")]
    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "Por gentileza, forneça um endereço de e-mail válido.")]
    public string Email { get; set; }

    [DisplayName("Usuário")]
    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    public string PasswordHash { get; set; }

    [DisplayName("Cadastro")]
    public DateTime DataCadastro { get; set; }

    [DisplayName("Alteração")]
    public DateTime DataAtualizacao { get; set; }

    [DisplayName("Nome")]
    public string NomeCompleto => $"{Nome} {Sobrenome}";

    public string CodigoRedefinicaoSenha { get; set; }

    public ICollection<UsuarioPapelViewModel> Papeis { get; set; }

    public bool Deletado { get; set; }
}