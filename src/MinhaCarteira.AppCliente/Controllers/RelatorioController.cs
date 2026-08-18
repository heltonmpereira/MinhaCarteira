using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel.Relatorio;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoGastos;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.EvolucaoSaldoPeriodo;
using MinhaCarteira.AppCliente.ViewModel.Relatorio.FluxoCaixa;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public async Task<IActionResult> Dashboard(int? ano = null, int[] meses = null, Guid? contaBancariaId = null)
    {
        var dataAtual = DateTime.Now;
        var anoSelecionado = ano ?? dataAtual.Year;

        var mesesSelecionados = (meses != null && meses.Length > 0)
            ? meses.Where(m => m >= 1 && m <= 12).Distinct().OrderBy(m => m).ToList()
            : new List<int> { dataAtual.Month };

        if (mesesSelecionados.Count == 0)
            mesesSelecionados.Add(dataAtual.Month);

        var model = new DashboardRelatorioViewModel
        {
            Ano = anoSelecionado,
            Mes = mesesSelecionados.Last(),
            Meses = mesesSelecionados,
            ContaBancariaId = contaBancariaId
        };

        // Carrega as contas bancárias
        var respostaContas = await ContaBancariaServico.Navegar(null, false);
        if (respostaContas.BemSucedido)
        {
            model.ContasBancarias = respostaContas.Dados;
        }

        // Mês principal (último selecionado) para manter EvolucaoSaldo e EvolucaoGastos legados
        var mesPrincipal = model.Mes;

        var respostaEvolucaoSaldo = await Servico.EvolucaoSaldo(anoSelecionado, mesPrincipal, contaBancariaId);
        if (respostaEvolucaoSaldo.BemSucedido)
            model.EvolucaoSaldo = respostaEvolucaoSaldo.Dados;

        // Estrutura Multi-Meses
        var evolucoesPorMes = new Dictionary<int, EvolucaoGastosViewModel>();
        var mesesVM = new List<EvolucaoGastosMesViewModel>();
        var cultura = CultureInfo.CurrentCulture;
        int ordem = 0;
        foreach (var mes in mesesSelecionados)
        {
            var resp = await Servico.EvolucaoGastos(anoSelecionado, mes, contaBancariaId);
            if (resp.BemSucedido && resp.Dados != null)
            {
                evolucoesPorMes[mes] = resp.Dados;
                mesesVM.Add(new EvolucaoGastosMesViewModel
                {
                    Ano = anoSelecionado,
                    Mes = mes,
                    Chave = $"{anoSelecionado}-{mes:D2}",
                    Nome = cultura.DateTimeFormat.GetMonthName(mes).ToUpperInvariant(),
                    Ordem = ordem++,
                    EhMaisRecente = mes == model.Mes
                });
            }
        }

        // Mantém a estrutura legada (para compatibilidade) se existir o mês principal
        if (evolucoesPorMes.TryGetValue(mesPrincipal, out var principal))
            model.EvolucaoGastos = principal;

        // Constrói o modelo unificado multi-mês
        model.EvolucaoGastosMultiMes = ConstruirModeloMultiMes(
            anoSelecionado,
            mesesSelecionados,
            mesesVM,
            evolucoesPorMes);

        return View(model);
    }

    private static EvolucaoGastosMultiMesViewModel ConstruirModeloMultiMes(
        int ano,
        List<int> mesesSelecionados,
        List<EvolucaoGastosMesViewModel> mesesVM,
        Dictionary<int, EvolucaoGastosViewModel> evolucoesPorMes)
    {
        var diasMax = mesesSelecionados.Max(m => DateTime.DaysInMonth(ano, m));
        var itens = new List<EvolucaoGastosMultiMesDiarioViewModel>(diasMax);
        var dataAtual = DateTime.Now;

        for (int dia = 1; dia <= diasMax; dia++)
        {
            var itemDia = new EvolucaoGastosMultiMesDiarioViewModel { Dia = dia };
            foreach (var mesVM in mesesVM)
            {
                var diasNoMes = DateTime.DaysInMonth(ano, mesVM.Mes);
                decimal? acumulado = null;
                decimal diario = 0;

                if (dia <= diasNoMes && evolucoesPorMes.TryGetValue(mesVM.Mes, out var evolucao) && evolucao.Itens != null)
                {
                    var dadoDia = evolucao.Itens.FirstOrDefault(i => i.Dia == dia);
                    if (dadoDia != null)
                    {
                        diario = dadoDia.GastosMesAtual;
                        acumulado = dadoDia.GastosAcumuladosMesAtual;

                        // Para meses que não são o corrente OU dias que já passaram no mês corrente, mantém o valor
                        // Para dias futuros do mês atual, retorna null (como na lógica original)
                        var ehMesCorrente = ano == dataAtual.Year && mesVM.Mes == dataAtual.Month;
                        if (ehMesCorrente && dia > dataAtual.Day)
                            acumulado = null;
                    }
                }

                itemDia.ValoresPorMes.Add(new EvolucaoGastosValorMesViewModel
                {
                    ChaveMes = mesVM.Chave,
                    Mes = mesVM.Mes,
                    Ano = ano,
                    NomeMes = mesVM.Nome,
                    GastosAcumulados = acumulado,
                    GastosDiarios = diario
                });
            }
            itens.Add(itemDia);
        }

        return new EvolucaoGastosMultiMesViewModel
        {
            Ano = ano,
            MesesSelecionados = mesesSelecionados,
            Meses = mesesVM,
            Itens = itens
        };
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

