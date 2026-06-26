using System;
using System.Collections.Generic;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class ContaBancaria : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public int Ordem { get; set; }
    public string Nome { get; set; }
    public string Agencia { get; set; }
    public string Conta { get; set; }
    public DateTime DataSaldoInicial { get; set; }
    public decimal ValorSaldoInicial { get; set; }
    public decimal ValorSaldoAtual { get; set; }
    public bool ExibirNaTelaInicial { get; set; }
    public bool Deletado { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public Guid InstituicaoFinanceiraId { get; set; }
    public InstituicaoFinanceira InstituicaoFinanceira { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }

    public ICollection<MovimentoBancario> Movimentos { get; set; }
}