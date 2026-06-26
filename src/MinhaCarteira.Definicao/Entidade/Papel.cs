using System;
using System.Collections.Generic;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class Papel : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Observacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }

    public ICollection<UsuarioPapel> Usuarios { get; set; }

    public bool Deletado { get; set; }
}