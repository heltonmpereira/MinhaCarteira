using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class Organizacao : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }

    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }

    public Guid? AdministradorId { get; set; }
    public Usuario Administrador { get; set; }

    //public virtual ICollection<Usuario> Usuarios { get; set; }

    public bool Deletado { get; set; }
}
