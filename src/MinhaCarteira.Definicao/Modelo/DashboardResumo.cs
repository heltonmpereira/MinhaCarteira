using MinhaCarteira.Definicao.Entidade;
using System.Collections.Generic;
using System.Linq;

namespace MinhaCarteira.Definicao.Modelo;

public class DashboardResumo
{
    [Newtonsoft.Json.JsonIgnore]
    public ICollection<ContaBancaria> ContasBancarias { get; set; }
    [Newtonsoft.Json.JsonIgnore]
    public ICollection<MovimentoBancario> Movimentos { get; set; }
    [Newtonsoft.Json.JsonIgnore]
    public ICollection<AgendamentoParcela> AgendamentoParcelas { get; set; }

    public int QuantidadeMovimentoBancarios => Movimentos.Count;
    public int QuantidadeAgendamentoParcelas => AgendamentoParcelas.Count;

    public decimal ValorSaldoAtual => ContasBancarias
        .Sum(s => s.ValorSaldoAtual);

    public decimal ReceitasPrevistas => AgendamentoParcelas
        .Where(w => w.Agendamento.Tipo == TipoMovimento.Credito)
        .Sum(w => w.Valor);
    public decimal DespesasPrevistas => AgendamentoParcelas
        .Where(w => w.Agendamento.Tipo == TipoMovimento.Debito)
        .Sum(w => w.Valor);

    public decimal ReceitasRealizadas => Movimentos
        .Where(w => w.TipoMovimento == TipoMovimento.Credito)
        .Sum(w => w.Valor);
    public decimal DespesasRealizadas => Movimentos
        .Where(w => w.TipoMovimento == TipoMovimento.Debito)
        .Sum(w => w.Valor);
}
