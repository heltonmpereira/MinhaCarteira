using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.Servico.Relatorio;
using System;
using System.Threading.Tasks;

namespace MinhaCarteira.AppServer.Controllers.Relatorio;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatorioController(IHttpContextAccessor httpContextAccessor, RelatorioServico servico) : ControllerBase
{
    protected RelatorioServico Servico { get; } = servico;
    protected IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;
    protected string IdUsuarioLogado { get; } = httpContextAccessor.HttpContext?.
            User
            .FindFirst("UsuarioId")?
            .Value;

    [HttpGet("fluxo-de-caixa/{ano:int}")]
    public virtual async Task<IActionResult> FluxoCaixa(int ano = 2024)
    {
        var retorno = await Servico.FluxoCaixa(ano, new System.Guid(IdUsuarioLogado));
        
        return !retorno.BemSucedido ? BadRequest() : Ok(retorno);
    }

    [HttpGet("evolucao-saldo/{ano:int}/{mes:int}")]
    public virtual async Task<IActionResult> EvolucaoSaldo(int ano, int mes, Guid? contaBancariaId = null)
    {
        var retorno = await Servico.EvolucaoSaldo(ano, mes, new System.Guid(IdUsuarioLogado), contaBancariaId);
        
        return !retorno.BemSucedido ? BadRequest() : Ok(retorno);
    }

    [HttpGet("evolucao-gastos/{ano:int}/{mes:int}")]
    public virtual async Task<IActionResult> EvolucaoGastos(int ano, int mes, Guid? contaBancariaId = null)
    {
        var retorno = await Servico.EvolucaoGastos(ano, mes, new System.Guid(IdUsuarioLogado), contaBancariaId);
        
        return !retorno.BemSucedido ? BadRequest() : Ok(retorno);
    }

    [HttpGet("evolucao-saldo-periodo")]
    public virtual async Task<IActionResult> EvolucaoSaldoPeriodo(DateTime dataInicial, DateTime dataFinal, Guid? contaBancariaId = null)
    {
        var retorno = await Servico.EvolucaoSaldoPeriodo(dataInicial, dataFinal, new System.Guid(IdUsuarioLogado), contaBancariaId);
        
        return !retorno.BemSucedido ? BadRequest() : Ok(retorno);
    }
}
