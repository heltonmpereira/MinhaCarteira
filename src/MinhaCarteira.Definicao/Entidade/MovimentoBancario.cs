using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Modelo;
using System;
using System.Collections.ObjectModel;

namespace MinhaCarteira.Definicao.Entidade;

public class MovimentoBancario : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public TipoMovimento TipoMovimento { get; set; }
    public DateTime DataMovimento { get; set; }
    public string Descricao { get; set; }
    public string Competencia { get; set; }
    public string Observacao { get; set; }
    public decimal Valor { get; set; }
    public decimal ValorReal =>
        TipoMovimento == TipoMovimento.Credito
            ? Valor
            : Valor * (-1);

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; }

    public Guid CentroClassificacaoId { get; set; }
    public CentroClassificacao CentroClassificacao { get; set; }

    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; }

    public Guid ContaBancariaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }

    public Guid? AgendamentoParcelaId { get; set; }
    public AgendamentoParcela AgendamentoParcela { get; set; }

    public Guid? TransferenciaBancariaId { get; set; }
    public TransferenciaBancaria TransferenciaBancaria { get; set; }

    public Guid? ImportarArquivoId { get; set; }
    public ImportarArquivo ImportarArquivo { get; set; }

    public Guid? ConciliacaoBancariaMovimentoId { get; set; }
    public ConciliacaoBancariaMovimento ConciliacaoBancaria { get; set; }

    public bool Deletado { get; set; }

    public Collection<MovimentoBancarioArquivo> MovimentoBancarioArquivos { get; set; }
}
