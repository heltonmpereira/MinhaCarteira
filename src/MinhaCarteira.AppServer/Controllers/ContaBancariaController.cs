using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using System;
using System.Threading.Tasks;

namespace MinhaCarteira.AppServer.Controllers;

public class ContaBancariaController(IContaBancariaServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<ContaBancaria, Guid, IContaBancariaServico, IContaBancariaRepositorio>(servico,
        httpContextAccessor)
{
    [Route("incrementar-prioridade/{id:guid}")]
    [HttpPost]
    public async Task<IActionResult> IncrementarPrioridade(Guid id) =>
        await Servico.RespostaServicoAsync<bool>(
            parameters: [id]);

    [Route("decrementar-prioridade/{id:guid}")]
    [HttpPost]
    public async Task<IActionResult> DecrementarPrioridade(Guid id) =>
        await Servico.RespostaServicoAsync<bool>(
            parameters: [id]);

    [Route("reativar/{id:guid}")]
    [HttpPost]
    public async Task<IActionResult> Reativar(Guid id) =>
    await Servico.RespostaServicoAsync<bool>(
        parameters: [id]);

    [Route("atualizar-saldos")]
    [HttpPost]
    public async Task<IActionResult> AtualizarSaldos() =>
        await Servico.RespostaServicoAsync<bool>(
            parameters: null);

    [Route("obter-contas-visiveis-na-tela-inicial")]
    [HttpPost]
    public async Task<IActionResult> ObterContasVisiveisNaTelaInicial()
    {
        var criterio = ObterFiltroInicial(null);
        criterio.AdicionarIncludes = true;
        criterio.Ordenacao = "Ordem";
        criterio.AdicionarFiltro("Sistema", new FiltroOpcao
        {
            NomePropriedade = nameof(ContaBancaria.ExibirNaTelaInicial),
            Operador = TipoOperadorBusca.Igual,
            Valor = true
        });
        criterio.AdicionarFiltro("Sistema", new FiltroOpcao
        {
            NomePropriedade = nameof(ContaBancaria.Deletado),
            Operador = TipoOperadorBusca.Igual,
            Valor = false
        });

        return await Servico.RespostaPaginadaServicoAsync<ContaBancaria>(criterio, null, nameof(Navegar));
    }

    //public override async Task<IActionResult> Navegar(string criterioJson, bool exibirDeletados = false)
    //{
    //    var grupoDeletado = new GrupoFiltro("Sistema", [
    //        new(){
    //            NomePropriedade = "Deletado",
    //            Operador = TipoOperadorBusca.Igual,
    //            Valor = false
    //        }
    //    ])
    //    { 
    //        RelacaoOutrosGrupos = TipoOperadorLogico.And
    //    };

    //    var filtro = base.ObterFiltroInicial(criterioJson);
    //    filtro.AdicionarGrupo(grupoDeletado);

    //    //return await base.Navegar(filtro);
    //    return await Servico.RespostaPaginadaServicoAsync<ContaBancaria>(filtro);
    //}
}
