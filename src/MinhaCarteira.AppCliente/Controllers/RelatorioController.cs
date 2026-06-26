using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel.Relatorio;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldoPeriodo;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.FluxoCaixa;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class RelatorioController(IRelatorioRefit servico, IContaBancariaRefit contaBancariaServico) : PadraoController
{
    protected IRelatorioRefit Servico { get; set; } = servico;
    protected IContaBancariaRefit ContaBancariaServico { get; set; } = contaBancariaServico;

    [HttpGet]
    public IActionResult FluxoCaixa() => View(new FluxoCaixaViewModel() { Ano = DateTime.Now.Year });

    [HttpPost]
    public async Task<IActionResult> FluxoCaixa(FluxoCaixaViewModel model)
    {
        var retorno = await Servico.FluxoCaixa(model.Ano);

        return View(retorno.Dados);
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(int? ano = null, int? mes = null, Guid? contaBancariaId = null)
    {
        var dataAtual = DateTime.Now;
        var anoSelecionado = ano ?? dataAtual.Year;
        var mesSelecionado = mes ?? dataAtual.Month;

        var model = new DashboardRelatorioViewModel
        {
            Ano = anoSelecionado,
            Mes = mesSelecionado,
            ContaBancariaId = contaBancariaId
        };

        // Carrega as contas bancárias
        var respostaContas = await ContaBancariaServico.Navegar(null, false);
        if (respostaContas.BemSucedido)
        {
            model.ContasBancarias = respostaContas.Dados;
        }

        // Carrega os dados via Refit (que já tem o token configurado)
        var respostaEvolucaoSaldo = await Servico.EvolucaoSaldo(anoSelecionado, mesSelecionado, contaBancariaId);
        var respostaEvolucaoGastos = await Servico.EvolucaoGastos(anoSelecionado, mesSelecionado, contaBancariaId);

        if (respostaEvolucaoSaldo.BemSucedido)
            model.EvolucaoSaldo = respostaEvolucaoSaldo.Dados;

        if (respostaEvolucaoGastos.BemSucedido)
            model.EvolucaoGastos = respostaEvolucaoGastos.Dados;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EvolucaoSaldoPeriodo(DateTime? dataInicial = null, DateTime? dataFinal = null, Guid? contaBancariaId = null)
    {
        var dataAtual = DateTime.Now;
        var dataInicialSelecionada = dataInicial ?? DateTime.Now;
        var dataFinalSelecionada = dataFinal ?? new DateTime(dataAtual.Year, dataAtual.Month, DateTime.DaysInMonth(dataAtual.Year, dataAtual.Month));

        var model = new EvolucaoSaldoPeriodoRelatorioViewModel
        {
            DataInicial = dataInicialSelecionada,
            DataFinal = dataFinalSelecionada,
            ContaBancariaId = contaBancariaId
        };

        // Carrega as contas bancárias
        var respostaContas = await ContaBancariaServico.Navegar(null, false);
        if (respostaContas.BemSucedido)
        {
            model.ContasBancarias = respostaContas.Dados;
        }

        // Carrega os dados via Refit
        var respostaEvolucaoSaldoPeriodo = await Servico.EvolucaoSaldoPeriodo(dataInicialSelecionada, dataFinalSelecionada, contaBancariaId);
        if (respostaEvolucaoSaldoPeriodo.BemSucedido)
        {
            model.EvolucaoSaldoPeriodo = respostaEvolucaoSaldoPeriodo.Dados;
        }

        return View(model);
    }
}
