using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.AppCliente.ViewModel;

public class MovimentoBancarioViewModel : BaseViewModel, IEntidade<Guid>
{
    public MovimentoBancarioViewModel()
    {
        var now = DateTime.Now;
        DataMovimento = new DateTime(now.Year, now.Month, now.Day);
        TipoMovimento = TipoMovimento.Debito;
        Competencia ??= $"{DataMovimento:yyyyMM}";
    }

    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    [DisplayName("Tipo")]
    public TipoMovimento TipoMovimento { get; set; }
    [DisplayName("Data")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
    public DateTime DataMovimento { get; set; }
    [DisplayName("Descrição")]
    public string Descricao { get; set; }
    [DisplayName("Competência")]
    public string Competencia { get; set; }
    [DisplayName("Observação")]
    public string Observacao { get; set; }
    [DataType(DataType.Currency)]
    public decimal Valor { get; set; }
    [DataType(DataType.Currency)]
    public decimal ValorReal =>
        TipoMovimento == TipoMovimento.Credito
            ? Valor
            : Valor * (-1);

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    [DisplayName("Pessoa")]
    public Guid PessoaId { get; set; }
    public PessoaViewModel Pessoa { get; set; }
    private string _nomePessoa;
    public string NomePessoa
    {
        get => Pessoa != null
            ? Pessoa.Nome
            : _nomePessoa;
        set => _nomePessoa = value;
    }

    [DisplayName("Centro de Classificação")]
    public Guid CentroClassificacaoId { get; set; }
    public CentroClassificacaoViewModel CentroClassificacao { get; set; }
    private string _nomeCentroClassificacao;
    public string NomeCentroClassificacao
    {
        get => CentroClassificacao != null
            ? CentroClassificacao.Nome
            : _nomeCentroClassificacao;
        set => _nomeCentroClassificacao = value;
    }

    [DisplayName("Categoria")]
    public Guid CategoriaId { get; set; }
    public CategoriaViewModel Categoria { get; set; }
    private string _caminhoCategoria;
    public string CaminhoCategoria
    {
        get => Categoria != null
            ? Categoria.Caminho
            : _caminhoCategoria;
        set => _caminhoCategoria = value;
    }

    [DisplayName("Conta Bancária")]
    public Guid ContaBancariaId { get; set; }
    public ContaBancariaViewModel ContaBancaria { get; set; }
    private string _nomeContaBancaria;
    public string NomeContaBancaria
    {
        get => ContaBancaria != null
            ? ContaBancaria.Nome
            : _nomeContaBancaria;
        set => _nomeContaBancaria = value;
    }

    public Guid? TransferenciaBancariaId { get; set; }
    public TransferenciaBancariaViewModel TransferenciaBancaria { get; set; }
    //public MovimentoBancarioTransferenciaViewModel TransferenciaBancaria { get; set; }

    public Guid? ConciliacaoBancariaMovimentoId { get; set; }

    public Guid ProprietarioId { get; set; }

    public bool Deletado { get; set; }

    public Collection<MovimentoBancarioArquivoViewModel> MovimentoBancarioArquivos { get; set; }
}
