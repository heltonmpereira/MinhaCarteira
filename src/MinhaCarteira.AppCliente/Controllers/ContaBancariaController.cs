using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Helper;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;


namespace MinhaCarteira.AppCliente.Controllers;

[BreadcrumbActionFilter]
public class ContaBancariaController : BaseController<ContaBancariaViewModel, Guid, IContaBancariaRefit>
{
    public ContaBancariaController(IContaBancariaRefit servico, IHttpContextAccessor httpContextAccessor)
        : base(servico, httpContextAccessor)
    {
        OrdenacaoPadrao = "Deletado, Ordem";
        ExibirRegistrosDeletados = true;
    }

    [HttpGet]
    public async Task<IActionResult> Reativar(Guid id)
    {
        return await this.ChamarServicoProsseguirIndex<bool, Guid, IContaBancariaRefit>(id);
    }

    [HttpGet]
    public async Task<IActionResult> IncrementarPrioridade(Guid id)
    {
        return await this.ChamarServicoProsseguirIndex<bool, Guid, IContaBancariaRefit>(id);
    }

    [HttpGet]
    public async Task<IActionResult> DecrementarPrioridade(Guid id)
    {
        return await this.ChamarServicoProsseguirIndex<bool, Guid, IContaBancariaRefit>(id);
    }

    [HttpGet]
    public async Task<IActionResult> AtualizarSaldos()
    {
        return await this.ChamarServicoProsseguirIndex<bool, Guid, IContaBancariaRefit>(null);
    }

    [HttpGet]
    public async Task<IActionResult> ImportarMovimentos(Guid id)
    {
        return await this.ChamarServicoView<ContaBancariaViewModel, Guid, IContaBancariaRefit>(
            model: id,
            apiMetodo: nameof(IContaBancariaRefit.ObterPorId));
    }
}
