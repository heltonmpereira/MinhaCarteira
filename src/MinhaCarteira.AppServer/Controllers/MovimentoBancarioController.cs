using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.AppServer.Controllers;

public class MovimentoBancarioController(IMovimentoBancarioServico servico, IHttpContextAccessor httpContextAccessor)
    : BaseApiController<MovimentoBancario, Guid, IMovimentoBancarioServico, IMovimentoBancarioRepositorio>(servico,
        httpContextAccessor)
{
    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir(MovimentoBancarioTransferencia item)
    {
        DefinirUsuarioLogado(item);
        return await Servico
            .RespostaServicoAsync<bool>([item]);
    }


    [Route("conciliar-movimento")]
    [HttpPost]
    public async Task<IActionResult> ConciliarMovimento(Guid idMovimento, string idParcelas) =>
        await Servico.RespostaServicoAsync<int>(
            parameters: [new Guid(IdUsuarioLogado), idMovimento, idParcelas]);
}
