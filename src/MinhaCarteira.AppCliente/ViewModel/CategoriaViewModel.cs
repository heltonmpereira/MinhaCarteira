using System;
using System.ComponentModel;
using MinhaCarteira.AppCliente.Models.Interface;
using MinhaCarteira.AppCliente.ViewModel.Base;

namespace MinhaCarteira.AppCliente.ViewModel;

public class CategoriaViewModel : BaseViewModel, IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public string Nome { get; set; }
    public string Caminho { get; set; }
    [DisplayName("Cadastro")]
    public DateTime DataCriacao { get; set; }
    [DisplayName("Alteração")]
    public DateTime? DataAlteracao { get; set; }
    [DisplayName("Ignorar movimentos bancários?")]
    public bool IgnorarMovimentacoes { get; set; }

    public Guid? ProprietarioId { get; set; }

    [DisplayName("Categoria pai")]
    public Guid? CategoriaPaiId { get; set; }
    public CategoriaViewModel CategoriaPai { get; set; }
    public string NomeCategoriaPai => CategoriaPai != null
        ? CategoriaPai.Caminho
        : string.Empty;

    public bool Deletado { get; set; }

    public Guid? IconeId { get; set; }
    public IconeViewModel Icone { get; set; }
}
