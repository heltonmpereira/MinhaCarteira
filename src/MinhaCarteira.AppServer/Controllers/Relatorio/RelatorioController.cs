using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Servico.Relatorio;

namespace MinhaCarteira.AppServer.Controllers.Relatorio;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatorioController(IHttpContextAccessor httpContextAccessor, RelatorioServico servico) : ControllerBase
{
    private Guid[] ConverterStringGuids(string guids)
    {
        Guid[] contasBancariasIds = null;
        if (!string.IsNullOrWhiteSpace(guids))
        {
            contasBancariasIds = [.. guids.Split(',')
                .Select(s => Guid.TryParse(s.Trim().Replace("\"", ""), out var guid) ? guid : Guid.Empty)
                .Where(g => g != Guid.Empty)];
        }

        return contasBancariasIds;
    }

    protected RelatorioServico Servico { get; } = servico;
    protected IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;
    protected string IdUsuarioLogado { get; } = httpContextAccessor.HttpContext?.
            User
            .FindFirst("UsuarioId")?
            .Value;

    [HttpGet("fluxo-de-caixa/{ano:int}")]
    public virtual async Task<IActionResult> FluxoCaixa(int ano = 2024)
    {
        var retorno = await Servico.FluxoCaixa(ano, new Guid(IdUsuarioLogado));

        return !retorno.BemSucedido ? BadRequest() : Ok(retorno);
    }

    [HttpGet("evolucao-saldo")]
    public virtual async Task<IActionResult> EvolucaoSaldo(TipoMovimento tipo, int[] anos, int[] meses, string txtContasBancariasIds)
    {
        Guid[] contasBancariasIds = ConverterStringGuids(txtContasBancariasIds);
        var retorno = await Servico.EvolucaoSaldo(
            tipo,
            anos,
            meses,
            new Guid(IdUsuarioLogado),
            contasBancariasIds);

        return !retorno.BemSucedido ? BadRequest() : Ok(retorno);

        //if (contaBancariaId.HasValue && (contasBancariasIds == null || contasBancariasIds.Length == 0))
        //    contasBancariasIds = [contaBancariaId.Value];



        //var retorno = contasBancariasIds != null && contasBancariasIds.Length > 0
        //    ? await Servico.Repositorio.GetEvolucaoSaldo(ano, mes, new Guid(IdUsuarioLogado), contasBancariasIds).ConfigureAwait(false)
        //    : await Servico.Repositorio.GetEvolucaoSaldo(ano, mes, new Guid(IdUsuarioLogado), contaBancariaId).ConfigureAwait(false);
        //
        //return !retorno.Equals(default) ? Ok(new RespostaServico<EvolucaoSaldo>(retorno)) : BadRequest();
    }

    /*
    [HttpGet("evolucao-saldo/{ano:int}/{mes:int}")]
    public virtual async Task<IActionResult> EvolucaoSaldo(int ano, int mes, Guid? contaBancariaId = null, string txtContasBancariasIds = null)
    {
        Guid[] contasBancariasIds = ConverterStringGuids(txtContasBancariasIds);
        if (contaBancariaId.HasValue && (contasBancariasIds == null || contasBancariasIds.Length == 0))
            contasBancariasIds = [contaBancariaId.Value];

        var retorno = contasBancariasIds != null && contasBancariasIds.Length > 0
            ? await Servico.Repositorio.GetEvolucaoSaldo(ano, mes, new Guid(IdUsuarioLogado), contasBancariasIds).ConfigureAwait(false)
            : await Servico.Repositorio.GetEvolucaoSaldo(ano, mes, new Guid(IdUsuarioLogado), contaBancariaId).ConfigureAwait(false);

        return !retorno.Equals(default) ? Ok(new RespostaServico<EvolucaoSaldo>(retorno)) : BadRequest();
    }

    [HttpGet("evolucao-gastos/{ano:int}/{mes:int}")]
    public virtual async Task<IActionResult> EvolucaoGastos(int ano, int mes, Guid? contaBancariaId = null, string txtContasBancariasIds = null)
    {
        Guid[] contasBancariasIds = ConverterStringGuids(txtContasBancariasIds);
        if (contaBancariaId.HasValue && (contasBancariasIds == null || contasBancariasIds.Length == 0))
            contasBancariasIds = new[] { contaBancariaId.Value };

        var retorno = contasBancariasIds != null && contasBancariasIds.Length > 0
            ? await Servico.Repositorio.GetEvolucaoGastos(ano, mes, new Guid(IdUsuarioLogado), contasBancariasIds).ConfigureAwait(false)
            : await Servico.Repositorio.GetEvolucaoGastos(ano, mes, new Guid(IdUsuarioLogado), contaBancariaId).ConfigureAwait(false);

        return !retorno.Equals(default) ? Ok(new RespostaServico<EvolucaoGastos>(retorno)) : BadRequest();
    }

    [HttpGet("evolucao-creditos/{ano:int}/{mes:int}")]
    public virtual async Task<IActionResult> EvolucaoCreditos(int ano, int mes, Guid? contaBancariaId = null, string txtContasBancariasIds = null)
    {
        Guid[] contasBancariasIds = ConverterStringGuids(txtContasBancariasIds);
        if (contaBancariaId.HasValue && (contasBancariasIds == null || contasBancariasIds.Length == 0))
            contasBancariasIds = new[] { contaBancariaId.Value };

        var retorno = contasBancariasIds != null && contasBancariasIds.Length > 0
            ? await Servico.Repositorio.GetEvolucaoCreditos(ano, mes, new Guid(IdUsuarioLogado), contasBancariasIds).ConfigureAwait(false)
            : await Servico.Repositorio.GetEvolucaoCreditos(ano, mes, new Guid(IdUsuarioLogado), contaBancariaId).ConfigureAwait(false);

        return !retorno.Equals(default) ? Ok(new RespostaServico<EvolucaoCreditos>(retorno)) : BadRequest();
    }

    [HttpGet("evolucao-saldo-periodo")]
    public virtual async Task<IActionResult> EvolucaoSaldoPeriodo(DateTime dataInicial, DateTime dataFinal, Guid? contaBancariaId = null, string txtContasBancariasIds = null)
    {
        Guid[] contasBancariasIds = ConverterStringGuids(txtContasBancariasIds);
        if (contaBancariaId.HasValue && (contasBancariasIds == null || contasBancariasIds.Length == 0))
            contasBancariasIds = new[] { contaBancariaId.Value };

        var retorno = contasBancariasIds != null && contasBancariasIds.Length > 0
            ? await Servico.Repositorio.GetEvolucaoSaldoPeriodo(dataInicial, dataFinal, new Guid(IdUsuarioLogado), contasBancariasIds).ConfigureAwait(false)
            : await Servico.Repositorio.GetEvolucaoSaldoPeriodo(dataInicial, dataFinal, new Guid(IdUsuarioLogado), contaBancariaId).ConfigureAwait(false);

        return !retorno.Equals(default) ? Ok(new RespostaServico<EvolucaoSaldoPeriodo>(retorno)) : BadRequest();
    }

    [HttpGet("gastos-por-categoria-periodo")]
    public virtual async Task<IActionResult> GastosPorCategoriaPeriodo(DateTime dataInicial, DateTime dataFinal, Guid? contaBancariaId = null, string txtContasBancariasIds = null)
    {
        Guid[] contasBancariasIds = ConverterStringGuids(txtContasBancariasIds);
        if (contaBancariaId.HasValue && (contasBancariasIds == null || contasBancariasIds.Length == 0))
            contasBancariasIds = new[] { contaBancariaId.Value };

        var retorno = contasBancariasIds != null && contasBancariasIds.Length > 0
            ? await Servico.Repositorio.GetGastosPorCategoriaPeriodo(dataInicial, dataFinal, new Guid(IdUsuarioLogado), contasBancariasIds).ConfigureAwait(false)
            : await Servico.Repositorio.GetGastosPorCategoriaPeriodo(dataInicial, dataFinal, new Guid(IdUsuarioLogado), contaBancariaId).ConfigureAwait(false);

        return !retorno.Equals(default) ? Ok(new RespostaServico<GastosPorCategoriaPeriodo>(retorno)) : BadRequest();
    }
    */
}
