using MinhaCarteira.Definicao.Interface.Entidade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaCarteira.Definicao.Entidade;

public class DashboardMonitor : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string CorFundo { get; set; }
    public string CriterioBuscaMovimentosJson { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public bool Deletado { get; set; }

    public Guid AgendamentoId { get; set; }
    public Agendamento Agendamento { get; set; }

    [NotMapped]
    public ICollection<MovimentoBancario> Movimentos { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }
}
