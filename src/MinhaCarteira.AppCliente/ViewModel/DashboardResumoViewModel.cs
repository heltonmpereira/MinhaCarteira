namespace MinhaCarteira.AppCliente.ViewModel;

public class DashboardResumoViewModel
{
    public int QuantidadeMovimentoBancarios { get; set; }
    public int QuantidadeAgendamentoParcelas { get; set; }

    public decimal ValorSaldoAtual { get; set; }

    public decimal ReceitasPrevistas { get; set; }
    public decimal DespesasPrevistas { get; set; }

    public decimal ReceitasRealizadas { get; set; }
    public decimal DespesasRealizadas { get; set; }
}
