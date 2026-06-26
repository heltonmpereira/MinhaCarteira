using System.Net.Http;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Modelo.Usuario;
using MinhaCarteira.Teste.AppServer.Base;
using MinhaCarteira.Teste.AppServer.Helper;
using Xunit;

namespace MinhaCarteira.Teste.AppServer.Testes;

[Collection("TestesDeIntegracao")]
public class LoginApiTest(TestFixture fixture)
{
    private readonly HttpClient _client = fixture.Client;

    [Theory]
    [InlineData("usuario", "invalido", "Usuário não localizado")]
    [InlineData("admin", "teste", "Usuário/Senha inválidos.")]
    [InlineData("admin", "admin", "Usuário localizado com sucesso.")]
    public async Task TestarLogin(string usuario, string senha, string resultadoEsperado)
    {
        var login = new UsuarioLogin() { Username = usuario, Password = senha };

        var rota = "/api/login";
        var retorno = await _client.EnviarPostAsync<UsuarioToken>(rota, login);
        Assert.Equal(resultadoEsperado, retorno.Mensagem);
    }

    [Theory]
    [InlineData("Nome da Organizçaão", "Teste", "Registro", "teste@teste.com", "teste", "teste", "Usuário registrado com sucesso!")]
    public async Task TestarRegistro(string nomeOrganizacao, string nome, string sobrenome, string email, string usuario, string senha, string resultadoEsperado)
    {
        var registro = new OrganizacaoRegistro
        {
            Nome = nomeOrganizacao,
            Administrador = new UsuarioRegistro
            {
                Nome = nome,
                Sobrenome = sobrenome,
                Email = email,
                Username = usuario,
                Password = senha
            }
        };

        var rota = "/api/login/registrar";
        var retorno = await _client.EnviarPostAsync<Usuario>(rota, registro);
        Assert.Equal(resultadoEsperado, retorno.Mensagem);

        retorno = await _client.EnviarPostAsync<Usuario>(rota, registro);
        Assert.Equal("Este nome de usuário ou endereço de e-mail já está sendo utilizado.", retorno.Mensagem);
    }

    [Fact]
    public async Task TestarAlteracaoSenha()
    {
        var registro = new OrganizacaoRegistro()
        {
            Nome = "Nome da Organização",
            Administrador = new UsuarioRegistro
            {
                Nome = "Teste",
                Sobrenome = "Registro",
                Email = "teste@registro.com",
                Username = "testeregistro",
                Password = "testeregistro"
            }
        };

        var rota = "/api/login/registrar";
        dynamic retorno = await _client.EnviarPostAsync<Organizacao>(rota, registro);
        Assert.Equal("Usuário registrado com sucesso!", retorno.Mensagem);

        var model = new AlterarSenha()
        {
            Id = retorno.Dados.Administrador.Id,
            SenhaAtual = "testeregistro",
            NovaSenha = "senhaAlterada"
        };

        rota = "/api/login/alterarSenha";
        var token = await _client.ObterTokenLogin("testeregistro", "testeregistro");
        retorno = await _client.EnviarPutAsync<bool>(rota, model, token);
        Assert.Equal("Alteração de senha realizada com sucesso.", retorno.Mensagem);

        var novoToken = await _client.ObterTokenLogin("testeregistro", "senhaAlterada");
        Assert.NotNull(novoToken);
    }
}