using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MinhaCarteira.AppServer.Model;

public interface IValidacaoPosseController
{
    Task<IActionResult> RegistroPertenceAoUsuarioLogadoAsync(object id, string caminhoPropriedade = null, bool adicionarIncludes = true, string nomeMetodo = "ObterPorId", Type tipo = default);
}

public interface IValidacaoPosseController<TPK> : IValidacaoPosseController
{
    Task<IActionResult> RegistroPertenceAoUsuarioLogadoAsync(TPK id, string caminhoPropriedade = null, bool adicionarIncludes = true, string nomeMetodo = "ObterPorId", Type tipo = default);

    Task<IActionResult> IValidacaoPosseController.RegistroPertenceAoUsuarioLogadoAsync(object id, string caminhoPropriedade, bool adicionarIncludes, string nomeMetodo, Type tipo)
        => RegistroPertenceAoUsuarioLogadoAsync((TPK)id, caminhoPropriedade, adicionarIncludes, nomeMetodo, tipo);
}