using System.Net;

namespace MinhaCarteira.Definicao.Interface.Servico.Resposta;

public interface IRespostaServico<T>
{
    HttpStatusCode? StatusCode { get; set; }
    string Mensagem { get; set; }
    bool BemSucedido { get; set; }
    string MensagemErro { get; set; }
    T Dados { get; set; }
}