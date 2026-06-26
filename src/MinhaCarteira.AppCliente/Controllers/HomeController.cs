using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaCarteira.AppCliente.Controllers;

[AllowAnonymous]
public class HomeController(IHomeRefit servico, IContaBancariaRefit contaBancariaServico)
    : PadraoController
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<PartialViewResult> PartialMonitores(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                id = DateTime.Now.ToString("yyyyMM");

            var monitorRequest = await servico.ObterDashboardMonitores(id);
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(id, monitorRequest.Dados, null);
            return PartialView("_PartialMonitores", retorno);
        }
        catch (ApiException apiEx)
        {
#if DEBUG
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, apiEx.Content);
#else
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, apiEx.Message);
#endif
            return PartialView("_PartialMonitores", retorno);
        }
        catch (Exception ex)
        {
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, ex.Message);
            return PartialView("_PartialMonitores", retorno);
        }
    }

    public async Task<PartialViewResult> PartialResumo(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                id = DateTime.Now.ToString("yyyyMM");

            var dashboardRequest = await servico.ObterDashboardResumo(id);
            var retorno = new Tuple<DashboardResumoViewModel, string>(dashboardRequest.Dados.FirstOrDefault(), id);
            return PartialView("_PartialResumo", retorno);
            //var retorno = new Tuple<string, IList<DashboardResumoViewModel>, string>(id, dashboardRequest.Dados, null);
            //return PartialView("_PartialMonitores", retorno);
        }
        catch (ApiException apiEx)
        {
#if DEBUG
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, apiEx.Content);
#else
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, apiEx.Message);
#endif
            return PartialView("_PartialMonitores", retorno);
        }
        catch (Exception ex)
        {
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, ex.Message);
            return PartialView("_PartialMonitores", retorno);
        }
    }

    public async Task<PartialViewResult> PartialContasBancarias()
    {
        try
        {
            var response = await contaBancariaServico.ObterContasVisiveisNaTelaInicial();
            var retorno = new Tuple<IList<ContaBancariaViewModel>, string>(response.Dados, null);
            return PartialView("_PartialContasBancarias", retorno);
        }
        catch (ApiException apiEx)
        {
#if DEBUG
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, apiEx.Content);
#else
            var retorno = new Tuple<string, IList<DashboardMonitorViewModel>, string>(null, null, apiEx.Message);
#endif
            return PartialView("_PartialContasBancarias", retorno);
        }
        catch (Exception ex)
        {
            var retorno = new Tuple<IList<ContaBancariaViewModel>, string>(null, ex.Message);
            return PartialView("_PartialContasBancarias", retorno);
        }
    }
}