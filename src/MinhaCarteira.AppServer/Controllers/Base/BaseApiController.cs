using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio.Base;
using MinhaCarteira.Definicao.Interface.Servico.Base;

namespace MinhaCarteira.AppServer.Controllers.Base;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController<TEntidade, TPK, TServico, TRepositorio>(TServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseLeituraApiController<TEntidade, TPK, TServico>(servico, httpContextAccessor), IValidacaoPosseController<TPK>
    where TEntidade : class, IEntidade<TPK>
    where TRepositorio : IRepositorio<TEntidade, TPK>
    where TServico : IServico<TEntidade, TPK, TRepositorio>
{

    [HttpPost]
    public virtual async Task<IActionResult> Incluir(TEntidade item)
    {
        AtribuirItemAoUsuarioLogado(item);
        return await Servico.RespostaServicoAsync<TEntidade>(parameters: [item]);
    }

    [HttpPut]
    [ValidarPosse]
    public async Task<IActionResult> Alterar(TEntidade item)
    {
        AtribuirItemAoUsuarioLogado(item);
        return await Servico.RespostaServicoAsync<TEntidade>(parameters: [item]);
    }

    [HttpDelete("{id}")]
    [ValidarPosse]
    public async Task<IActionResult> Deletar(TPK id) => 
        await Servico.RespostaServicoAsync<int>(parameters: [id]);

    [HttpDelete("DeletarRange")]
    [ValidarPosse]
    public async Task<IActionResult> DeletarRange(TPK[] ids) =>
        await Servico.RespostaServicoAsync<int>(parameters: [ids]);
}
