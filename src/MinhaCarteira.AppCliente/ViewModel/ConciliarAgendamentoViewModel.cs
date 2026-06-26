using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.AppCliente.ViewModel;

public class ConciliarAgendamentoViewModel
{
    public ConciliarAgendamentoViewModel() { }

    public ConciliarAgendamentoViewModel(
        AgendamentoParcelaViewModel parcela)
    {
        ContaBancariaId = parcela.ContaBancariaId ?? parcela.Agendamento.ContaBancariaId;
        NomeContaBancaria = parcela.NomeContaBancaria ?? parcela.NomeContaBancaria;

        CategoriaId = parcela.Agendamento?.CategoriaId;
        CaminhoCategoria = parcela.Agendamento?.CaminhoCategoria;

        Parcela = parcela;

        DataInicial = (parcela.DataPagamento ?? parcela.Data).AddDays(-3);
        DataFinal = (parcela.DataPagamento ?? parcela.Data).AddDays(3);

        ValorInicial = 0;
        ValorFinal = (parcela.ValorPago ?? parcela.Valor) + 10;
    }

    public ConciliarAgendamentoViewModel(
        MovimentoBancarioViewModel movimento)
    {
        ContaBancariaId = movimento.ContaBancariaId;
        NomeContaBancaria = movimento.NomeContaBancaria;

        CategoriaId = movimento.CategoriaId;
        CaminhoCategoria = movimento.CaminhoCategoria;

        Movimento = movimento;

        DataInicial = (movimento.DataMovimento).AddDays(-3);
        DataFinal = (movimento.DataMovimento).AddDays(3);

        ValorInicial = 0;
        ValorFinal = movimento.Valor * (decimal)1.1;
    }

    public AgendamentoParcelaViewModel Parcela { get; set; }
    public MovimentoBancarioViewModel Movimento { get; set; }

    public string NomeContaBancaria { get; set; }
    public Guid? ContaBancariaId { get; set; }
    public DateTime DataInicial { get; set; }
    public DateTime DataFinal { get; set; }
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal ValorInicial { get; set; }
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal ValorFinal { get; set; }
    public string Descricao { get; set; }

    [DisplayName("Categoria")]
    public Guid? CategoriaId { get; set; }
    public CategoriaViewModel Categoria { get; set; }
    private string _caminhoCategoria;
    public string CaminhoCategoria
    {
        get => Categoria != null
            ? Categoria.Caminho
            : _caminhoCategoria;
        set => _caminhoCategoria = value;
    }

}
