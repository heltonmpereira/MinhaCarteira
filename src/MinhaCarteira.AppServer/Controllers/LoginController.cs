using Dhani.Utilitarios.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.AppServer.Controllers.Base;
using MinhaCarteira.AppServer.Helper;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Definicao.Modelo.Usuario;
using MinhaCarteira.Servico.Helper;
using System;
using System.Threading.Tasks;

namespace MinhaCarteira.AppServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController(ILoginServico servico, IHttpContextAccessor httpContextAccessor)
    : PadraoApiController<ILoginServico>(servico, httpContextAccessor)
{
    [AllowAnonymous]
    [HttpPost()]
    public async Task<IActionResult> Login(UsuarioLogin loginInfo)
    {
        var retorno = await Servico.RespostaServicoAsync<UsuarioToken>(
             parameters: [loginInfo]);

        return retorno;
    }

    [AllowAnonymous]
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(OrganizacaoRegistro registro)
    {
        var org = new Organizacao
        {
            Nome = registro.Nome,
            Administrador = new Usuario().Mapear(registro.Administrador)
        };
        org.Administrador.PasswordHash = registro.Administrador.Password.CriptografarTexto();
        org.Administrador.Organizacao = org;

        var retorno = await Servico.RespostaServicoAsync<Organizacao>(
             parameters: [org]);

        return retorno;
    }

    [AllowAnonymous]
    [HttpPost("redefinirSenha")]
    public async Task<IActionResult> RedefinirSenha(UsuarioSolicitacaoRedefinicaoSenha model)
    {
        var retorno = await Servico.RespostaServicoAsync<UsuarioRedefinirSenha>(
             parameters: [model]);

        return retorno;
    }

    [AllowAnonymous]
    [HttpPost("alterarSenhaComCodigoRedefinicao")]
    public async Task<IActionResult> AlterarSenhaComCodigoRedefinicao(UsuarioRedefinirSenha model)
    {
        model.Password = model.Password.CriptografarTexto();
        var retorno = await Servico.RespostaServicoAsync<bool>(
             parameters: [model]);

        return retorno;
    }

    [HttpPut("alterarSenha")]
    public async Task<IActionResult> AlterarSenha(AlterarSenha model) =>
        await Servico.RespostaServicoAsync<bool>(
             parameters: [model]);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var resposta = await Servico.RespostaServicoAsync<Usuario>(
            parameters: [id, true]);

        return ConfrontarUsuarioLogadoEProprietario<Usuario>(resposta);
        //if (resposta is OkObjectResult retornoOk)
        //{
        //    var resp = retornoOk.Value as IRespostaServico<Usuario>;
        //    if (resp.Dados.Id != new Guid(IdUsuarioLogado))
        //    {
        //        var retorno = new RespostaServico<Usuario>(null, "Falha ao validar informações do usuário")
        //        {
        //            BemSucedido = false,
        //            StatusCode = HttpStatusCode.BadRequest,
        //            MensagemErro = "Falha ao validar informações do usuário"
        //        };

        //        return BadRequest(retorno);
        //    }
        //}

        //return resposta;
    }

    [HttpPut]
    public async Task<IActionResult> Alterar(Usuario item)
    {
        if (item.Id.ToString() != IdUsuarioLogado)
        {
            var erro = new Exception("Dados inválidos");
            var retorno = new RespostaServico<Exception>(erro, erro.Message);
            return BadRequest(retorno);
        }

        return await Servico.RespostaServicoAsync<Usuario>(
            parameters: [item]);
    }

    [HttpGet("acessar-como/{id}")]
    public async Task<IActionResult> AcessarComo(Guid id) =>
        await Servico.RespostaServicoAsync<UsuarioToken>(
            parameters: [id]);
}