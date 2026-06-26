using System;
using Microsoft.AspNetCore.Diagnostics;
using Refit;

namespace MinhaCarteira.AppCliente.Models;

public class ErrorViewModel
{
    private string ListarMensagemExcecoesInternas(Exception innerException)
    {
        if (innerException == null)
            return string.Empty;
        else return innerException.Message + "\r\n"
                + ListarMensagemExcecoesInternas(innerException?.InnerException);
    }

    public ErrorViewModel()
    {

    }
    public ErrorViewModel(IExceptionHandlerPathFeature handle)
    {
        Mensagem = handle?.Error.Message;
        StackTrace = handle?.Error.StackTrace;
        MensagensInternas = ListarMensagemExcecoesInternas(handle?.Error?.InnerException);

        if (handle?.Error is ApiException ex)
            MensagemApi = !string.IsNullOrWhiteSpace(ex.Content)
                ? ex.Content
                : ex.Message;
    }

    public string Caminho { get; set; }
    public string Mensagem { get; set; }
    public string MensagemApi { get; set; }
    public string MensagensInternas { get; set; }
    public string StackTrace { get; set; }
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}