
using System;
using System.Threading.Tasks;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.ViewModel;
using Refit;

namespace MinhaCarteira.AppCliente.Refit;

public interface IAuditoriaRefit
{
    [Get("")]
    Task<RespostaPaginadaServico<AuditoriaViewModel>> Navegar(string criterioJson, bool exibirRegistrosDeletados);

    [Get("/{id}")]
    Task<RespostaServico<AuditoriaViewModel>> ObterPorId(Guid id);
}
