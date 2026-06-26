using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class Auditoria : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public bool Deletado { get; set; }
    
    public DateTime DataHora { get; set; }
    public string Acao { get; set; }
    public string Entidade { get; set; }
    public string EntidadeId { get; set; }
    public string DadosAntigos { get; set; }
    public string DadosNovos { get; set; }
    public string IpUsuario { get; set; }
    public string UserAgent { get; set; }
    public string Rotina { get; set; }
    
    public Guid? UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    
    public Guid? OrganizacaoId { get; set; }
    public Organizacao Organizacao { get; set; }
}
