using System.Net;

namespace MinhaCarteira.AppCliente.Models.Interface.Resposta;

public interface IRespostaServico<T>
{
    HttpStatusCode? StatusCode { get; set; }
    string Mensagem { get; set; }
    bool BemSucedido { get; set; }
    string MensagemErro { get; set; }
    T Dados { get; set; }
}