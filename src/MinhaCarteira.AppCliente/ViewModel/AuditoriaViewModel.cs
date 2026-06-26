
using System;
using System.ComponentModel;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class AuditoriaViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public bool Deletado { get; set; }

    [DisplayName("Data e Hora")]
    public DateTime DataHora { get; set; }

    public string Acao { get; set; }
    public string Entidade { get; set; }

    [DisplayName("Entidade ID")]
    public string EntidadeId { get; set; }

    [DisplayName("Dados Antigos")]
    public string DadosAntigos { get; set; }

    [DisplayName("Dados Novos")]
    public string DadosNovos { get; set; }

    [DisplayName("IP do Usuário")]
    public string IpUsuario { get; set; }

    [DisplayName("User Agent")]
    public string UserAgent { get; set; }

    public string Rotina { get; set; }

    [DisplayName("Usuário ID")]
    public Guid? UsuarioId { get; set; }
    public UsuarioViewModel Usuario { get; set; }

    [DisplayName("Organização ID")]
    public Guid? OrganizacaoId { get; set; }
}
