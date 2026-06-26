using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.AppServer.Model;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;

namespace MinhaCarteira.AppServer.Controllers;

public class PapelController(IPapelServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<Papel, Guid, IPapelServico, IPapelRepositorio>(servico, httpContextAccessor)
{
    protected override TipoFiltroRegistroEnum FiltroRegistros { get; set; } = TipoFiltroRegistroEnum.NaoFiltrarUsuario;

    [HttpPut("atualizar-usuarios")]
    public async Task<IActionResult> AtualizarUsuarios(Guid id, Guid[] idsUsuario) =>
        await Servico.RespostaServicoAsync<Papel>(
            parameters: [id, idsUsuario]);

}