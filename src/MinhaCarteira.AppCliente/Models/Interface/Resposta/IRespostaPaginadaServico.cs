using System.Collections.Generic;

namespace MinhaCarteira.AppCliente.Models.Interface.Resposta;

public interface IRespostaPaginadaServico<T> : IRespostaServico<IList<T>>
{
    int ItensPorPagina { get; set; }
    int NumeroPagina { get; set; }
    int TotalPaginas { get; set; }
    int TotalRegistros { get; set; }
}