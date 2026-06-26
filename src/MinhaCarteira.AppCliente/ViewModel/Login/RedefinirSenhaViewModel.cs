using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MinhaCarteira.AppCliente.Attribute;

namespace MinhaCarteira.AppCliente.ViewModel.Login;

public class RedefinirSenhaViewModel
{
    public Guid Id { get; set; }
    public string CodigoRedefinicaoSenha { get; set; }


    [DisplayName("Senha")]
    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo {1} caracteres.")]
    [MaxLength(12, ErrorMessage = "A senha deve ter no máximo {1} caracteres.")]
    [HasAtLeastOneLowerCaseLetter]
    [HasAtLeastOneUpperCaseLetter]
    [HasAtLeastOneNumber]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DisplayName("Confirmar senha")]
    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Os campos {0} e {1} devem ser iguais")]
    public string ConfirmarPassword { get; set; }

}