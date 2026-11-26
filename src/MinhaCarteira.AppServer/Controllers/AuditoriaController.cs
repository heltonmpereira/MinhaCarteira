using System;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;

namespace MinhaCarteira.AppServer.Controllers;

[Authorize(Roles = "superadmin,admin")]
[ApiController]
[Route("api/[controller]")]
public class AuditoriaController(IAuditoriaServico servico, IHttpContextAccessor httpContextAccessor) 
    : PadraoApiController<IAuditoriaServico>(servico, httpContextAccessor)

{
    protected override bool IgnorarValidacaoPosse => UsuarioEhSuperAdmin;
    protected override string PropriedadeValidacaoPosse => "OrganizacaoId";
    protected override string IdValidacaoPosse => IdOrganizacaoUsuarioLogado;
    protected override TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.FiltrarPorOrganizacaoLogada;

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
