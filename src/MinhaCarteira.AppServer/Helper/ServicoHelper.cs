using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dhani.Utilitarios.Filtro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.AppServer.Helper;

public static class ServicoHelper
{
    public static OkResult Ok() => new();
    public static BadRequestResult BadRequest() => new();
    public static StatusCodeResult StatusCode([ActionResultStatusCode] int statusCode) => new(statusCode);
    public static OkObjectResult Ok([ActionResultObjectValue] object value)
        => new(value);
    public static BadRequestObjectResult BadRequest([ActionResultObjectValue] object error)
        => new(error);
    public static BadRequestObjectResult BadRequest([ActionResultObjectValue] object error, HttpStatusCode? statusCode)
        => new(error) { StatusCode = (int)(statusCode ?? HttpStatusCode.BadRequest) };
    public static ObjectResult StatusCode([ActionResultStatusCode] int statusCode, [ActionResultObjectValue] object value)
    {
        return new ObjectResult(value)
        {
            StatusCode = statusCode
        };
    }

    public static async Task<IActionResult> RespostaServicoAsync<TRetorno>(
        this object servico,
        object[] parameters,
        [CallerMemberName] string nomeMetodo = null)
    {
        return await servico.RespostaServicoDiretoAsync<TRetorno>(
            nomeMetodo: nomeMetodo,
            parameters: parameters);
    }

    private static async Task<IActionResult> RespostaServicoDiretoAsync<TRetorno>(
        this object servico,
        string nomeMetodo,
        object[] parameters)
    {
        IActionResult resposta;
        try
        {
            var metodo = servico.GetType().GetMethod(nomeMetodo) ??
                throw new Exception($"Não foi possível localizar o método '{nomeMetodo}' na API '{servico.GetType().Name}'.");

            var task = (Task)metodo.Invoke(servico, parameters);

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            var resultado = (IRespostaServico<TRetorno>)resultProperty.GetValue(task);

            resposta = resultado == null || !resultado.BemSucedido
                ? BadRequest(resultado, resultado?.StatusCode ?? HttpStatusCode.InternalServerError)
                : Ok(resultado);
        }
        catch (Exception e)
        {
            resposta = StatusCode(
                (int)HttpStatusCode.InternalServerError,
                new RespostaServico<Exception>(e, e.Message));
        }

        resposta?.DefinirCodigoStatus<TRetorno>();
        return resposta;
    }

    public static async Task<IActionResult> RespostaPaginadaServicoAsync<TRetorno>(
                this object servico,
                ICriterio criterio = null,
                object[] parameters = null,
                [CallerMemberName] string nomeMetodo = null)
    {
        return await servico.RespostaPaginadaServicoDiretoAsync<TRetorno>(
            nomeMetodo,
            criterio,
            parameters);
    }

    private static async Task<IActionResult> RespostaPaginadaServicoDiretoAsync<TRetorno>(
        this object servico,
        string nomeMetodo,
        ICriterio criterio,
        object[] parameters)
    {
        var metodo = servico.GetType().GetMethod(nomeMetodo);

        IActionResult resposta;
        try
        {
            criterio ??= new FiltroBase();
            parameters ??= [criterio];

            var task = (Task)metodo.Invoke(servico, parameters);

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            var resultado = (IRespostaPaginadaServico<TRetorno>)resultProperty.GetValue(task);

            var item = resultado ??
                new RespostaPaginadaServico<TRetorno>(dados: null);

            resposta = item.BemSucedido
                ? Ok(item)
                : BadRequest(item);
        }
        catch (Exception e)
        {
            resposta = StatusCode(
                (int)HttpStatusCode.InternalServerError,
                new RespostaPaginadaServico<Exception>(
                    dados: [e],
                    numeroPagina: 0,
                    itensPorPagina: 1,
                    totalRegistros: 0,
                    mensagem: e.Message));
        }

        resposta?.DefinirCodigoStatus<IList<TRetorno>>();
        return resposta;
    }
}