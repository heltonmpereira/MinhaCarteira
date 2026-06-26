using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Models.Interface.Resposta;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Controllers;

public class DashboardMonitorController(IDashboardMonitorRefit servico, IHttpContextAccessor httpContextAccessor)
    : BaseController<DashboardMonitorViewModel, Guid, IDashboardMonitorRefit>(servico, httpContextAccessor)
{
    protected override string OrdenacaoPadrao { get; set; } = "Titulo";

    public override Task InicializarObjeto(object instancia)
    {
        var monitor = instancia switch
        {
            IRespostaServico<DashboardMonitorViewModel> resposta => resposta.Dados,
            DashboardMonitorViewModel model => model,
            _ => null
        };

        if (monitor != null)
            monitor.Criterio = new ViewModel.Base.CriterioViewModel()
            {
                Colunas = ObterColunasFiltro(typeof(MovimentoBancarioViewModel))
            };

        return Task.CompletedTask;
    }
}
