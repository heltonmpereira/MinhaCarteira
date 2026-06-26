using System.Collections.Generic;
using MinhaCarteira.Definicao.Interface.Servico.Resposta;

namespace MinhaCarteira.Definicao.Modelo;

public class RespostaPaginadaServico<T>(
    IList<T> dados,
    int numeroPagina = 0,
    int itensPorPagina = 1,
    int totalRegistros = 0,
    string mensagem = null)
    :
        RespostaServico<IList<T>>(dados, mensagem),
        IRespostaPaginadaServico<T>
{
    public int NumeroPagina { get; set; } = numeroPagina;
    public int ItensPorPagina { get; set; } = itensPorPagina;
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; } = totalRegistros;
}