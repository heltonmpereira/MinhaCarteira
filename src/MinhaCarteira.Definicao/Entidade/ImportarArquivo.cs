using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class ImportarArquivo : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid ContaBancariaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; }
    public bool Deletado { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }
}