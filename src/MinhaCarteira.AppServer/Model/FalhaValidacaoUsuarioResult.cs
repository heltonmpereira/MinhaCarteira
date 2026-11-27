using System.Net;
using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.AppServer.Model;

/// <summary>
/// Resposta 403 com o corpo padrão da API, para o cliente exibir a mensagem.
/// (ForbidResult responde sem corpo; por isso herdamos de ObjectResult.)
/// </summary>
public class FalhaValidacaoUsuarioResult : ObjectResult
{
    public const string MensagemPadrao = "Falha ao validar informacoes do usuario";

    public FalhaValidacaoUsuarioResult(string mensagem = MensagemPadrao)
        : base(new RespostaServico<Usuario>(null, mensagem)
        {
            BemSucedido = false,
            Mensagem = mensagem,
            MensagemErro = mensagem,
            StatusCode = HttpStatusCode.Forbidden
        })
    {
        StatusCode = (int)HttpStatusCode.Forbidden;
    }
}
