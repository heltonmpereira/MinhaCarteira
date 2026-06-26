using System;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using Dhani.Utilitarios.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Helper;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using Newtonsoft.Json;

namespace MinhaCarteira.AppServer.Controllers;

public class AgendamentoController(IAgendamentoServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<Agendamento, Guid, IAgendamentoServico, IAgendamentoRepositorio>(servico, httpContextAccessor)
{
    private ICriterio FiltroAgendamento(string criterioJson)
    {
        var criterio = string.IsNullOrWhiteSpace(criterioJson)
            ? null
            : JsonConvert.DeserializeObject<FiltroBase>(criterioJson);

        criterio?.RenderizarValor();
        criterio.AdicionarFiltroProprietario(IdUsuarioLogado, "Agendamento.ProprietarioId");

        return criterio;
    }

    [Route("contas-a-vencer")]
    [HttpPost]
    public async Task<IActionResult> ContasAVencer([FromBody] FiltroBase criterioJson, bool exibirRegistrosDeletados) =>
        await Servico.RespostaPaginadaServicoAsync<AgendamentoParcela>(
            criterio: FiltroAgendamento(criterioJson.ToJson()));

    [Route("obter-parcela/{id:guid}")]
    [HttpGet]
    public async Task<IActionResult> ObterParcelaPorId(Guid id) =>
        await Servico.RespostaServicoAsync<AgendamentoParcela>(
            parameters: [id]);

    [Route("alterar-parcela")]
    [HttpPost]
    public async Task<IActionResult> AlterarParcela([FromBody] AgendamentoParcela parcela) =>
        await Servico.RespostaServicoAsync<AgendamentoParcela>(
            parameters: [parcela]);

    [Route("deletar-parcela/{id:guid}")]
    [HttpPost]
    public async Task<IActionResult> DeletarParcela(Guid id) =>
        await Servico.RespostaServicoAsync<int>(
            parameters: [id]);

    [Route("pagar-parcela")]
    [HttpPost]
    public async Task<IActionResult> PagarParcela([FromBody] AgendamentoParcela parcela) =>
        await Servico.RespostaServicoAsync<AgendamentoParcela>(
            parameters: [parcela]);

    [Route("restaurar-condicao-parcela")]
    [HttpPost]
    public async Task<IActionResult> RestaurarCondicaoParcela(Guid idParcela) =>
        await Servico.RespostaServicoAsync<int>(
            parameters: [idParcela]);

    [Route("conciliar-parcela")]
    [HttpPost]
    public async Task<IActionResult> ConciliarParcela(Guid idParcela, string idMovimentos) =>
        await Servico.RespostaServicoAsync<int>(
            parameters: [new Guid(IdUsuarioLogado), idParcela, idMovimentos]);

    [Route("conciliar-parcelas-a-partir-dashboard-monitor")]
    [HttpPost]
    public async Task<IActionResult> ConciliarParcelasAPartirDashboardMonitor(string mesANo) =>
        await Servico.RespostaServicoAsync<bool>(
            parameters: new[] { mesANo });

    [Route("estender-parcelas-5-anos/{id:guid}")]
    [HttpPost]
    public async Task<IActionResult> EstenderParcelasPor5Anos(Guid id) =>
        await Servico.RespostaServicoAsync<Agendamento>(
            parameters: [id]);
}
