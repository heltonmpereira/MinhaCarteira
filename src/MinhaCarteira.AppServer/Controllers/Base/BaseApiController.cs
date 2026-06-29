
using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;
using MinhaCarteira.Definicao.Interface.Servico.Base;
using System.Threading.Tasks;

namespace MinhaCarteira.AppServer.Controllers.Base;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController<TEntidade, TPK, TServico, TRepositorio>(TServico servico, IHttpContextAccessor httpContextAccessor) 
    : PadraoApiController<TServico>(servico, httpContextAccessor) 
    where TEntidade : class, IEntidade<TPK> 
    where TRepositorio : IRepositorio<TEntidade, TPK> 
    where TServico : IServico<TEntidade, TPK, TRepositorio>
{
    [HttpGet]
    public virtual async Task<IActionResult> Navegar(string criterioJson, bool exibirRegistrosDeletados = false)
    {
        var filtroInicial = ObterFiltroInicial(criterioJson);

        if (!exibirRegistrosDeletados)
        {
            var grupoRegistrosDeletados = new GrupoFiltro("Deletados", [
                new FiltroOpcao() {
                    NomePropriedade = "Deletado",
                    Valor = false,
                    Operador = TipoOperadorBusca.Igual,
                    RelacaoOutrosFiltros = TipoOperadorLogico.And
                }
            ])
            {
                RelacaoOutrosGrupos = TipoOperadorLogico.And
            };

            filtroInicial.AdicionarGrupo(grupoRegistrosDeletados);
        }

        return await Servico.RespostaPaginadaServicoAsync<TEntidade>(criterio: filtroInicial);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(TPK id)
    {
        var resposta = await Servico.RespostaServicoAsync<TEntidade>(parameters: [id, true]);
        return ConfrontarUsuarioLogadoEProprietario<TEntidade>(resposta);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Incluir(TEntidade item)
    {
        DefinirUsuarioLogado(item);
        return await Servico.RespostaServicoAsync<TEntidade>(parameters: [item]);
    }

    [HttpPut]
    public async Task<IActionResult> Alterar(TEntidade item)
    {
        DefinirUsuarioLogado(item);
        return await Servico.RespostaServicoAsync<TEntidade>(parameters: [item]);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(TPK id)
        => await Servico.RespostaServicoAsync<int>(parameters: [id]);

    [HttpDelete("DeletarRange")]
    public async Task<IActionResult> DeletarRange(TPK[] ids)
        => await Servico.RespostaServicoAsync<int>(parameters: [ids]);
}
