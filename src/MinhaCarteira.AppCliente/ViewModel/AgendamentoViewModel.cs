using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MinhaCarteira.AppCliente.Models;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class AgendamentoViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public TipoMovimento Tipo { get; set; }
    [DisplayName("Data")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
    public DateTime DataInicial { get; set; } = DateTime.Now.Date;
    [DisplayName("Descrição")]
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public string RegraRecorrencia { get; set; }
    [DisplayName("Parcelas")]
    public int QuantidadeParcelas { get; set; } = 1;
    public int Intervalo { get; set; } = 1;
    [DisplayName("Tipo de Parcelas")]
    public TipoParcelas TipoParcelas { get; set; } = TipoParcelas.Unica;
    [DisplayName("Tipo de Recorrência")]
    public TipoRecorrencia TipoRecorrencia { get; set; } = TipoRecorrencia.Mensal;
    [DisplayName("Observação")]
    public string Observacao { get; set; }
    [DisplayName("Despesa opcional")]
    public bool DespesaOpcional { get; set; }

    public Collection<AgendamentoParcelaViewModel> Parcelas { get; set; } = [];

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

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

    [DisplayName("Pessoa")]
    public Guid? PessoaId { get; set; }
    public PessoaViewModel Pessoa { get; set; }
    private string _nomePessoa;
    public string NomePessoa
    {
        get => Pessoa != null
            ? Pessoa.Nome
            : _nomePessoa;
        set => _nomePessoa = value;
    }

    [DisplayName("Conta Bancária")]
    public Guid? ContaBancariaId { get; set; }
    public ContaBancariaViewModel ContaBancaria { get; set; }
    private string _nomeContaBancaria;
    public string NomeContaBancaria
    {
        get => ContaBancaria != null
            ? ContaBancaria.Nome
            : _nomeContaBancaria;
        set => _nomeContaBancaria = value;
    }

    public Guid ProprietarioId { get; set; }

    public bool Deletado { get; set; }
}
