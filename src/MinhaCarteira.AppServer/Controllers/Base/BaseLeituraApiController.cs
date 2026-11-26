using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.AppServer.Controllers.Base;

public abstract class BaseLeituraApiController<TEntidade, TPK, TServico>(TServico servico, IHttpContextAccessor httpContextAccessor)
    : PadraoApiController<TServico>(servico, httpContextAccessor)
    where TEntidade : class, IEntidade<TPK>
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
    public virtual async Task<IActionResult> ObterPorId(TPK id)
    {
        var resposta = await Servico.RespostaServicoAsync<TEntidade>(parameters: [id, true]);
        return ConfrontarUsuarioLogadoEProprietario<TEntidade>(resposta);
    }
}