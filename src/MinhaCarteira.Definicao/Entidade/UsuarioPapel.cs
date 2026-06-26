using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class UsuarioPapel : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public Guid PapelId { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataCadastro { get; set; }

    public Papel Papel { get; set; }
    public Usuario Usuario { get; set; }

    public bool Deletado { get; set; }
}