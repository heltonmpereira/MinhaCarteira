using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoGastos;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldo;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldoPeriodo;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.FluxoCaixa;
using Refit;
using System;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Refit;

public interface IRelatorioRefit
{
    [Get("/fluxo-de-caixa/{ano}")]
    Task<RespostaServico<FluxoCaixaViewModel>> FluxoCaixa(int ano);

    [Get("/evolucao-saldo/{ano}/{mes}")]
    Task<RespostaServico<EvolucaoSaldoViewModel>> EvolucaoSaldo(int ano, int mes, Guid? contaBancariaId = null);

    [Get("/evolucao-gastos/{ano}/{mes}")]
    Task<RespostaServico<EvolucaoGastosViewModel>> EvolucaoGastos(int ano, int mes, Guid? contaBancariaId = null);

    [Get("/evolucao-saldo-periodo")]
    Task<RespostaServico<EvolucaoSaldoPeriodoViewModel>> EvolucaoSaldoPeriodo(DateTime dataInicial, DateTime dataFinal, Guid? contaBancariaId = null);
}
