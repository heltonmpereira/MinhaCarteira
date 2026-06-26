
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;
using Dhani.Utilitarios.Filtro;
using System.Threading.Tasks;

namespace MinhaCarteira.AppServer.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LogController(ILogServico servico, IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    protected ILogServico Servico { get; } = servico;
    protected IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;

    [HttpGet]
    public virtual async Task<IActionResult> Navegar(string criterioJson, bool exibirRegistrosDeletados = false)
    {
        var criterio = string.IsNullOrWhiteSpace(criterioJson)
            ? new FiltroBase()
            : Newtonsoft.Json.JsonConvert.DeserializeObject<FiltroBase>(criterioJson);

        criterio?.RenderizarValor();

        if (!exibirRegistrosDeletados)
        {
            var grupoRegistrosDeletados = new GrupoFiltro("Deletados", [
                new FiltroOpcao(){
                    NomePropriedade = "Deletado",
                    Valor = false,
                    Operador = TipoOperadorBusca.Igual,
                    RelacaoOutrosFiltros = TipoOperadorLogico.And
                }
            ])
            {
                RelacaoOutrosGrupos = TipoOperadorLogico.And
            };

            criterio.AdicionarGrupo(grupoRegistrosDeletados);
        }

        // Filter by OrganizacaoId
        var organizacaoIdClaim = HttpContextAccessor.HttpContext?.User.FindFirst("OrganizacaoId")?.Value;
        if (!string.IsNullOrEmpty(organizacaoIdClaim))
        {
            var grupoOrganizacao = new GrupoFiltro("Organizacao", new[]
            {
                new FiltroOpcao
                {
                    NomePropriedade = "OrganizacaoId",
                    Valor = organizacaoIdClaim,
                    Operador = TipoOperadorBusca.Igual,
                    RelacaoOutrosFiltros = TipoOperadorLogico.And
                }
            });
            criterio.AdicionarGrupo(grupoOrganizacao);
        }

        return await Servico.RespostaPaginadaServicoAsync<Log>(
            criterio: criterio);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        return await Servico.RespostaServicoAsync<Log>(
            parameters: [id, true]);
    }
}
