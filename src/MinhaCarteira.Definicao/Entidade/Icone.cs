using MinhaCarteira.Definicao.Interface.Entidade;
using System;

namespace MinhaCarteira.Definicao.Entidade;

public class Icone : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public bool Deletado { get; set; }
    public DateTime DataCadastro { get; set; }
    public Guid? ProprietarioId { get; set; }

    public string Nome { get; set; }
    public string Extensao { get; set; }
    public string Conteudo { get; set; }
}
