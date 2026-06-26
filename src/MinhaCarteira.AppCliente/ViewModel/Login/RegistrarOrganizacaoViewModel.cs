using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.AppCliente.ViewModel.Login;

public class RegistrarOrganizacaoViewModel
{
    [Required(ErrorMessage = "O preenchimento do campo {0} é obrigatório.")]
    [DisplayName("Nome da organização")]
    public string Nome { get; set; }

    public RegistrarUsuarioViewModel Administrador { get; set; }
}
