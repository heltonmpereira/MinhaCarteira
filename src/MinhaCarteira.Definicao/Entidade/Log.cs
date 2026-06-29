using System;
using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.Definicao.Entidade;

public class Log : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public bool Deletado { get; set; }

    public DateTime DataHora { get; set; }
    public TipoLogEnum TipoLog { get; set; }
    public string Categoria { get; set; }
    public string Mensagem { get; set; }
    public string DadosSerializados { get; set; }
    public string StackTrace { get; set; }
    public string IpUsuario { get; set; }
    public string UserAgent { get; set; }
    public string Url { get; set; }
    public string MetodoHttp { get; set; }
    public int? StatusCode { get; set; }

    public Guid? UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    public Guid? OrganizacaoId { get; set; }
    public Organizacao Organizacao { get; set; }
}
