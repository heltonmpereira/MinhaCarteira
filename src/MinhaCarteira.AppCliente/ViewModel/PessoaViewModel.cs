using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class PessoaViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public string Nome { get; set; }
    [DisplayName("Cliente?")]
    public bool EhCliente { get; set; }
    [DisplayName("Fornecedor?")]
    public bool EhFornecedor { get; set; }
    public string NomeExtrato { get; set; }
    [DisplayName("Ignorar movimentos bancários?")]
    public bool IgnorarMovimentacoes { get; set; }

    [DisplayName("Cadastro")]
    public DateTime DataCriacao { get; set; }
    [DisplayName("Alteração")]
    public DateTime? DataAlteracao { get; set; }

    public Guid ProprietarioId { get; set; }

    [DisplayName("Categoria")]
    public Guid? CategoriaId { get; set; }
    public CategoriaViewModel Categoria { get; set; }
    public IEnumerable<SelectListItem> Categorias { get; set; }
    private string caminhoCategoria;
    public string CaminhoCategoria
    {
        get => Categoria != null
            ? Categoria.Caminho
            : caminhoCategoria;
        set => caminhoCategoria = value;
    }

    [DisplayName("Centro de classificação")]
    public Guid? CentroClassificacaoId { get; set; }
    public CentroClassificacaoViewModel CentroClassificacao { get; set; }
    public IEnumerable<SelectListItem> CentrosClassificacao { get; set; }
    private string nomeCentroClassificacao;
    public string NomeCentroClassificacao
    {
        get => CentroClassificacao != null
            ? CentroClassificacao.Nome
            : nomeCentroClassificacao;
        set => nomeCentroClassificacao = value;
    }

    public bool Deletado { get; set; }
}
