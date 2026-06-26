using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.AppCliente.ViewModel.Login;

public class LoginViewModel
{
    [DisplayName("Usuário")]
    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    public string Username { get; set; }

    [DisplayName("Senha")]
    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DisplayName("Manter-me conectado")]
    public bool ManterConectado { get; set; }

    public string ReturnUrl { get; set; }
}