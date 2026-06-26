using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Definicao.Modelo.Usuario;
using Newtonsoft.Json;

namespace MinhaCarteira.Teste.AppServer.Helper;

public static class Utils
{
    private static StringContent PrepararChamada(this HttpClient client, object parameter, string token = null)
    {
        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return new StringContent(
            JsonConvert.SerializeObject(parameter),
            Encoding.Default,
            "application/json");
    }
    public static async Task<T> ProcessarRetorno<T>(this HttpResponseMessage resposta)
    {
        var stringResponse = await resposta.Content.ReadAsStringAsync();
        var retorno = System.Text.Json.JsonSerializer.Deserialize<T>(
            stringResponse,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        return retorno;
    }

    public static async Task<RespostaPaginadaServico<T>> EnviarGetAsync<T>(this HttpClient client, string rota, string token = null)
    {
        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resposta = await client.GetAsync(rota);
        return await resposta.ProcessarRetorno<RespostaPaginadaServico<T>>();
    }
    public static async Task<RespostaServico<T>> EnviarDeleteAsync<T>(this HttpClient client, string rota, string token = null)
    {
        if (!string.IsNullOrWhiteSpace(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resposta = await client.DeleteAsync(rota);
        return await resposta.ProcessarRetorno<RespostaServico<T>>();
    }
    public static async Task<RespostaServico<T>> EnviarPostAsync<T>(this HttpClient client, string rota, object parameter, string token = null)
    {
        var content = client.PrepararChamada(parameter, token);
        var resposta = await client.PostAsync(rota, content);
        return await resposta.ProcessarRetorno<RespostaServico<T>>();
    }
    public static async Task<RespostaServico<T>> EnviarPutAsync<T>(this HttpClient client, string rota, object parameter, string token = null)
    {
        var content = client.PrepararChamada(parameter, token);
        var resposta = await client.PutAsync(rota, content);
        return await resposta.ProcessarRetorno<RespostaServico<T>>();
    }

    public static async Task<string> ObterTokenLogin(this HttpClient client, string usuario, string senha)
    {
        var login = new UsuarioLogin() { Username = usuario, Password = senha };

        var rota = "/api/login";
        var retorno = await client.EnviarPostAsync<UsuarioToken>(rota, login);

        return retorno.Dados?.TokenAcesso;
    }
}