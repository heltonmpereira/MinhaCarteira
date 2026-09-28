using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppCliente.Controllers.Base;
using MinhaCarteira.AppCliente.Filter;
using MinhaCarteira.AppCliente.Helper;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Refit;
using MinhaCarteira.AppCliente.ViewModel;
using Newtonsoft.Json;
using Refit;


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

    [HttpPost]
    public async Task<JsonResult> Reordenar([FromBody] Guid[] idsOrdenados)
    {
        try
        {
            var retorno = await Servico.Reordenar(idsOrdenados);
            return Json(new
            {
                sucesso = retorno.BemSucedido,
                mensagem = retorno.Mensagem,
                mensagemErro = retorno.MensagemErro
            });
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HttpContext.Response.StatusCode = (int)ex.StatusCode;
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Sessão expirada. Faça login novamente."
                });
            }

            var retornoApi = await ex.GetContentAsAsync<RespostaServico<dynamic>>();
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(new
            {
                sucesso = false,
                mensagem = retornoApi?.Mensagem ?? "Falha ao reordenar as contas.",
                mensagemErro = retornoApi?.MensagemErro ?? ex.Message
            });
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            return Json(new
            {
                sucesso = false,
                mensagem = "Erro interno ao reordenar as contas.",
                mensagemErro = ex.Message
            });
        }
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
