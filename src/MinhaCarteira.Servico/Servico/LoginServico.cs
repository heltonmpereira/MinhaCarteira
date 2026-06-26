using System;
using System.Linq;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Interface.Servico;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Definicao.Modelo.Usuario;
using MinhaCarteira.Servico.Helper;
using MinhaCarteira.Servico.Servico.Base;
using Newtonsoft.Json;

namespace MinhaCarteira.Servico.Servico;

public class LoginServico(IUsuarioRepositorio repositorio, IRegistroAuditoriaServico registroAuditoriaServico)
    : BaseServico<Usuario, Guid, IUsuarioRepositorio>(repositorio), ILoginServico
{
    private readonly IRegistroAuditoriaServico _registroAuditoriaServico = registroAuditoriaServico;

    private static FiltroOpcao FiltroUsuario(string username) =>
        new("username", TipoOperadorBusca.Igual, username);
    private static FiltroOpcao FiltroEmail(string email) =>
        new("email", TipoOperadorBusca.Igual, email);

    private async Task<Usuario> ObterPorUsuario(string username)
    {
        var criterio = new FiltroBase();
        criterio.AdicionarFiltro(null, FiltroUsuario(username));

        return await BuscarUsuario(criterio);
    }
    private async Task<Usuario> ObterPorUsernameEEmail(string username, string email)
    {
        var criterio = new FiltroBase();
        criterio.AdicionarFiltro(null, FiltroUsuario(username));
        criterio.AdicionarFiltro(null, FiltroEmail(email));

        return await BuscarUsuario(criterio);
    }
    private async Task<Usuario> ObterPorUsernameOuEmail(string username, string email)
    {
        var criterio = new FiltroBase();
        criterio.AdicionarFiltro(null, FiltroUsuario(username));
        var filtroEmail = FiltroEmail(email);
        filtroEmail.RelacaoOutrosFiltros = TipoOperadorLogico.Or;
        criterio.AdicionarFiltro(null, filtroEmail);

        return await BuscarUsuario(criterio);
    }
    private async Task<Usuario> BuscarUsuario(ICriterio criterio)
    {
        var retorno = await Repositorio.Navegar(criterio).ConfigureAwait(false);
        var usuario = retorno.Item1.SingleOrDefault();

        return usuario;
    }

    public async Task<IRespostaServico<UsuarioToken>> Login(UsuarioLogin userInfo)
    {
        var usuario = await ObterPorUsuario(userInfo.Username).ConfigureAwait(false);
        if (usuario == null)
        {
            // Log failed login attempt
            await _registroAuditoriaServico.RegistrarAuditoriaAsync(
                "POST",
                "Login",
                null,
                null,
                JsonConvert.SerializeObject(new { Username = userInfo.Username, Password = "***REDACTED***" }),
                null,
                null,
                "Login/Login",
                null,
                Guid.Empty // We don't have organizacaoId yet for failed attempts
            );
            return new RespostaServico<UsuarioToken>(null, "Usuário não localizado")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.NotAcceptable
            };
        }

        if (!usuario.PasswordHash.VerificarHashSenha(userInfo.Password))
        {
            // Log failed login attempt
            await _registroAuditoriaServico.RegistrarAuditoriaAsync(
                "POST",
                "Login",
                null,
                null,
                JsonConvert.SerializeObject(new { Username = userInfo.Username, Password = "***REDACTED***" }),
                null,
                null,
                "Login/Login",
                usuario.Id,
                usuario.OrganizacaoId
            );
            return new RespostaServico<UsuarioToken>(null, "Usuário/Senha inválidos.")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.NotAcceptable
            };
        }

        var token = new UsuarioToken(usuario)
        {
            Roles = usuario.Papeis.Select(s => s.Papel.Nome).ToList(),
        };
        token.Roles.Add("UsuarioLogado");
        token.DataExpericaoToken = DateTime.Now.AddMonths(1);
        token.TokenAcesso = TokenServico.GerarToken(token, usuario.OrganizacaoId);

        // Log successful login
        await _registroAuditoriaServico.RegistrarAuditoriaAsync(
            "POST",
            "Login",
            null,
            null,
            JsonConvert.SerializeObject(new { Username = userInfo.Username, Password = "***REDACTED***", Sucesso = true }),
            null,
            null,
            "Login/Login",
            usuario.Id,
            usuario.OrganizacaoId
        );

        return new RespostaServico<UsuarioToken>(
            token,
            "Usuário localizado com sucesso.");
    }

    public async Task<IRespostaServico<UsuarioToken>> AcessarComo(Guid id)
    {
        var userDb = await ObterPorId(id).ConfigureAwait(false);
        if (userDb == null || userDb.Dados == null)
        {
            return new RespostaServico<UsuarioToken>(null, "Usuário não localizado")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.NotAcceptable
            };
        }

        var usuario = userDb.Dados;
        var token = new UsuarioToken(usuario)
        {
            Roles = usuario.Papeis.Select(s => s.Papel.Nome).ToList(),
        };
        token.TokenAcesso = TokenServico.GerarToken(token, usuario.OrganizacaoId);

        return new RespostaServico<UsuarioToken>(
            token,
            "Usuário localizado com sucesso.");
    }

    public async Task<IRespostaServico<UsuarioRedefinirSenha>> RedefinirSenha(UsuarioSolicitacaoRedefinicaoSenha model)
    {
        var usuario = await ObterPorUsernameEEmail(model.Username, model.Email).ConfigureAwait(false);
        if (usuario == null)
        {
            return new RespostaServico<UsuarioRedefinirSenha>(null, "Usuário não localizado")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.NotAcceptable
            };
        }

        var item = await Repositorio.RedefinirSenha(usuario).ConfigureAwait(false);
        var retorno = new RespostaServico<UsuarioRedefinirSenha>(
            item,
            "Solicitação de redefinição de senha realizada com sucesso.");

        return retorno;
    }

    public async Task<IRespostaServico<bool>> AlterarSenhaComCodigoRedefinicao(UsuarioRedefinirSenha model)
    {
        var usuario = await Repositorio.ObterPorId(model.Id);
        if (usuario == null)
        {
            return new RespostaServico<bool>(false, "Usuário não localizado")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.NotAcceptable
            };
        }

        usuario.PasswordHash = model.Password;
        usuario.CodigoRedefinicaoSenha = null;
        _ = await Repositorio.AlterarSenhaComCodigoRedefinicao(usuario).ConfigureAwait(false);
        var retorno = new RespostaServico<bool>(
            true,
            "Solicitação de redefinição de senha realizada com sucesso.");

        return retorno;
    }

    public async Task<IRespostaServico<bool>> AlterarSenha(AlterarSenha model)
    {
        var usuario = await Repositorio.ObterPorId(model.Id);
        if (usuario == null)
        {
            return new RespostaServico<bool>(false, "Usuário não localizado")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.NotAcceptable
            };
        }

        if (!usuario.PasswordHash.VerificarHashSenha(model.SenhaAtual))
        {
            return new RespostaServico<bool>(false, "Usuário/Senha inválidos.")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.NotAcceptable
            };
        }

        usuario.CodigoRedefinicaoSenha = null;
        usuario.PasswordHash = model.NovaSenha.CriptografarTexto();
        usuario.CodigoRedefinicaoSenha = null;
        _ = await Repositorio.AlterarSenha(usuario).ConfigureAwait(false);
        var retorno = new RespostaServico<bool>(
            true,
            "Alteração de senha realizada com sucesso.");

        return retorno;
    }

    public async Task<IRespostaServico<Organizacao>> Registrar(Organizacao organizacao)
    {
        var cadastrado = await ObterPorUsernameOuEmail(organizacao.Administrador.Username, organizacao.Administrador.Email);
        if (cadastrado != null)
        {
            return new RespostaServico<Organizacao>(null, "Este nome de usuário ou endereço de e-mail já está sendo utilizado.")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.Unauthorized
            };
        }

        var organizacaoDb = await Repositorio.Registrar(organizacao).ConfigureAwait(false); ;
        var retorno = new RespostaServico<Organizacao>(
            organizacaoDb,
            "Usuário registrado com sucesso!");

        return retorno;
    }

    public override async Task<IRespostaServico<Usuario>> Incluir(Usuario item)
    {
        var cadastrado = await ObterPorUsernameOuEmail(item.Username, item.Email);
        if (cadastrado != null)
        {
            return new RespostaServico<Usuario>(null, "Este nome de usuário ou endereço de e-mail já está sendo utilizado.")
            {
                BemSucedido = false,
                StatusCode = System.Net.HttpStatusCode.Unauthorized
            };
        }

        var retorno = await base.Incluir(item);
        retorno.Mensagem = !retorno.BemSucedido
            ? retorno.Mensagem
            : "Usuário registrado com sucesso!";

        return retorno;
    }
}