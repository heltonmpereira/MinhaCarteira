using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Usuario;
using MinhaCarteira.AppCliente.ViewModel;
using MinhaCarteira.AppCliente.ViewModel.Login;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface ILoginRefit
{
    [Get("/{id}")]
    Task<RespostaServico<UsuarioViewModel>> ObterPorId(Guid id);

    [Put("")]
    Task<RespostaServico<UsuarioViewModel>> Alterar(UsuarioViewModel item);

    [Post("/")]
    Task<RespostaServico<UsuarioToken>> Logar(LoginViewModel item);

    [Post("/registrar")]
    Task<RespostaServico<UsuarioViewModel>> Registrar(RegistrarOrganizacaoViewModel item);

    [Post("/RedefinirSenha")]
    Task<RespostaServico<RedefinirSenhaViewModel>> RedefinirSenha(SolicitarRedefinicaoSenhaViewModel model);

    [Post("/AlterarSenhaComCodigoRedefinicao")]
    Task<RespostaServico<bool>> AlterarSenhaComCodigoRedefinicao(RedefinirSenhaViewModel model);

    [Put("/AlterarSenha")]
    Task<RespostaServico<bool>> AlterarSenha(AlterarSenhaViewModel item);

    [Get("/acessar-como/{id}")]
    Task<RespostaServico<UsuarioToken>> AcessarComo(Guid id);
}