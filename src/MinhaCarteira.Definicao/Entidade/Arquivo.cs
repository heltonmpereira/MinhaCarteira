using MinhaCarteira.Definicao.Interface.Entidade;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaCarteira.Definicao.Entidade;

public class Arquivo : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public bool Deletado { get; set; }
    public Guid? ProprietarioId { get; set; }
    public DateTime DataCadastro { get; set; }

    public string SubPasta { get; set; }
    public string Nome { get; set; }
    public string Extensao { get; set; }

    [NotMapped]
    public string Conteudo { get; set; }
}