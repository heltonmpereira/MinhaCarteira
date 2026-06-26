using System;
using MinhaCarteira.AppCliente.Models.Interface.Resposta;
using Newtonsoft.Json;

namespace MinhaCarteira.AppCliente.Models;

public class RespostaServico<T> : IRespostaServico<T>
{
    private string ObterExceptionMaisProfunda(Exception erro = null)
    {
        if (erro != null)
            return erro.InnerException == null
                ? erro.Message
                : ObterExceptionMaisProfunda(erro.InnerException);

        if (Dados is Exception ex)
            return ObterExceptionMaisProfunda(ex);

        if (Dados is Exception[] exA)
            return ObterExceptionMaisProfunda(exA[0]);

        return string.Empty;
    }

    public RespostaServico() { }

    public RespostaServico(T dados, string mensagem = null)
    {
        Dados = dados;
        Mensagem = mensagem ?? (
            dados is Exception erro
                ? erro.Message
                : string.Empty);
        MensagemErro = dados is Exception || dados is Exception[]
            ? ObterExceptionMaisProfunda()
            : string.Empty;
        BemSucedido = dados is not Exception && dados is not Exception[];
    }

    //Propriedades precisam ter GET e SET públicos para a deserialização do cliente
    [JsonProperty(Order = 0)]
    public System.Net.HttpStatusCode? StatusCode { get; set; }

    [JsonProperty(Order = 1)]
    public string Mensagem { get; set; }

    [JsonProperty(Order = 2)]
    public bool BemSucedido { get; set; }

    [JsonProperty(Order = 3)]
    public string MensagemErro { get; set; }

    [JsonProperty(Order = 99)]
    public T Dados { get; set; }
}