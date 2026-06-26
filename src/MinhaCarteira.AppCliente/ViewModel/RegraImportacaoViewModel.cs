using System;
using System.ComponentModel;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class RegraImportacaoViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    [DisplayName("Palavras chaves - separadas por ;")]
    public string PalavrasChave { get; set; }
    [DisplayName("Valor Mínimo")]
    public decimal? ValorMinimo { get; set; }
    [DisplayName("Valor Máximo")]
    public decimal? ValorMaximo { get; set; }
    [DisplayName("Descrição")]
    public string Descricao { get; set; }

    [DisplayName("Conta Bancária (opcional)")]
    public Guid? ContaBancariaId { get; set; }
    public ContaBancariaViewModel ContaBancaria { get; set; }
    public string NomeContaBancaria => ContaBancaria?.Nome;

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
    public string NomeCentroClassificacao => CentroClassificacao?.Nome;

    [DisplayName("Pessoa")]
    public Guid PessoaId { get; set; }
    public PessoaViewModel Pessoa { get; set; }
    public string NomePessoa => Pessoa?.Nome;

    public Guid ProprietarioId { get; set; }
    public UsuarioViewModel Proprietario { get; set; }

    public bool Deletado { get; set; }
}
