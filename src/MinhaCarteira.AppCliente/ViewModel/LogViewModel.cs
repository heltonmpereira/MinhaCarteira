
using System;
using System.ComponentModel;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class LogViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public bool Deletado { get; set; }

    [DisplayName("Data e Hora")]
    public DateTime DataHora { get; set; }

    [DisplayName("Tipo de Log")]
    public TipoLogEnum TipoLog { get; set; }

    public string Categoria { get; set; }
    public string Mensagem { get; set; }

    [DisplayName("Dados Serializados")]
    public string DadosSerializados { get; set; }

    [DisplayName("Stack Trace")]
    public string StackTrace { get; set; }

    [DisplayName("IP do Usuário")]
    public string IpUsuario { get; set; }

    [DisplayName("User Agent")]
    public string UserAgent { get; set; }

    public string Url { get; set; }

    [DisplayName("Método HTTP")]
    public string MetodoHttp { get; set; }

    [DisplayName("Status Code")]
    public int? StatusCode { get; set; }

    [DisplayName("Usuário ID")]
    public Guid? UsuarioId { get; set; }
    public UsuarioViewModel Usuario { get; set; }

    [DisplayName("Organização ID")]
    public Guid? OrganizacaoId { get; set; }
}
