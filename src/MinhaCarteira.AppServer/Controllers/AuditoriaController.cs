using System;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;

namespace MinhaCarteira.AppServer.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuditoriaController(IAuditoriaServico servico, IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    protected IAuditoriaServico Servico { get; } = servico;
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

        // Filter by OrganizacaoId (commented out so admins see all)
        //var organizacaoIdClaim = HttpContextAccessor.HttpContext?.User.FindFirst("OrganizacaoId")?.Value;
        //if (!string.IsNullOrEmpty(organizacaoIdClaim))
        //{
        //    var grupoOrganizacao = new GrupoFiltro("Organizacao", new[]
        //    {
        //        new FiltroOpcao
        //        {
        //            NomePropriedade = "OrganizacaoId",
        //            Valor = organizacaoIdClaim,
        //            Operador = TipoOperadorBusca.Igual,
        //            RelacaoOutrosFiltros = TipoOperadorLogico.And
        //        }
        //    });
        //    criterio.AdicionarGrupo(grupoOrganizacao);
        //}

        return await Servico.RespostaPaginadaServicoAsync<Auditoria>(
            criterio: criterio);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        return await Servico.RespostaServicoAsync<Auditoria>(
            parameters: [id, true]);
    }
}
