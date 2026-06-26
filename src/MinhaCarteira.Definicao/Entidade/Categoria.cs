using System;
using MinhaCarteira.Definicao.Interface.Entidade;

namespace MinhaCarteira.Definicao.Entidade;

public class Categoria : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public string Nome { get; set; }
    //public string Icone { get; set; }
    public string Caminho { get; set; }
    //public string NomeArquivo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public bool IgnorarMovimentacoes { get; set; }
    public bool Deletado { get; set; }

    public Guid? CategoriaPaiId { get; set; }
    public Categoria CategoriaPai { get; set; }

    public Guid? ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }

    public Guid? IconeId { get; set; }
    public Icone Icone { get; set; }

    public string CaminhoRecursivo => GetCaminho(this);

    private static string GetCaminho(Categoria obj) =>
        obj.CategoriaPai == null
            ? obj.Nome
            : $"{GetCaminho(obj.CategoriaPai)} | {obj.Nome}";
}