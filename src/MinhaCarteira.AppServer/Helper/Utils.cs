using Microsoft.AspNetCore.Mvc;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;

namespace MinhaCarteira.AppServer.Helper;

public static class Utils
{
    public static void DefinirCodigoStatus<T>(ref IActionResult resposta)
    {
        resposta.DefinirCodigoStatus<T>();
    }

    public static void DefinirCodigoStatus<T>(this IActionResult resposta)
    {
        var objResult = (ObjectResult)resposta;

        if (objResult is not { Value: IRespostaServico<T> resp })
            return;

        if (objResult.StatusCode != null)
            resp.StatusCode ??= (System.Net.HttpStatusCode)objResult.StatusCode;

        if (resp.StatusCode >= System.Net.HttpStatusCode.BadRequest
            || resp.Mensagem == "A busca não encontrou nenhum registro.")
            resp.BemSucedido = false;
    }
}