using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MinhaCarteira.AppServer.Model;

public interface IValidacaoPosseController
{
    Task<IActionResult> RegistroPertenceAoUsuarioLogadoAsync(object id, string caminhoPropriedade = null, string nomeMetodo = "ObterPorId");
}

public interface IValidacaoPosseController<TPK> : IValidacaoPosseController
{
    Task<IActionResult> RegistroPertenceAoUsuarioLogadoAsync(TPK id, string caminhoPropriedade = null, string nomeMetodo = "ObterPorId");

    Task<IActionResult> IValidacaoPosseController.RegistroPertenceAoUsuarioLogadoAsync(object id, string caminhoPropriedade, string nomeMetodo)
        => RegistroPertenceAoUsuarioLogadoAsync((TPK)id, caminhoPropriedade, nomeMetodo);
}