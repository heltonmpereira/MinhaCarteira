using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Modelo;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MinhaCarteira.Definicao.Entidade;

public class Agendamento : IEntidade<Guid>
{
    public Guid Id { get; set; }
    public string IdAuxiliar { get; set; }
    public TipoMovimento Tipo { get; set; }
    public DateTime DataInicial { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public string RegraRecorrencia { get; set; }
    public int QuantidadeParcelas { get; set; }
    public int Intervalo { get; set; } = 1;
    public TipoParcelas TipoParcelas { get; set; }
    public TipoRecorrencia TipoRecorrencia { get; set; }
    public string Observacao { get; set; }
    public bool Deletado { get; set; }
    public bool DespesaOpcional { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; }

    public Guid CentroClassificacaoId { get; set; }
    public CentroClassificacao CentroClassificacao { get; set; }

    public Guid? PessoaId { get; set; }
    public Pessoa Pessoa { get; set; }

    public Guid? ContaBancariaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; }

    public Guid ProprietarioId { get; set; }
    public Usuario Proprietario { get; set; }

    public Collection<AgendamentoParcela> Parcelas { get; set; } = [];

    public void AdicionarParcela(DateTime data, int numeroParcela)
    {
        var parcela = new AgendamentoParcela()
        {
            AgendamentoId = Id,
            Data = data,
            Valor = Valor,
            PessoaId = PessoaId,
            ContaBancariaId = ContaBancariaId,
            NumeroParcela = numeroParcela
        };

        Parcelas.Add(parcela);
    }

    public void AtualizarParcelasEmAberto(AgendamentoParcela config)
    {
        Categoria = null;
        ContaBancaria = null;
        Pessoa = null;
        CentroClassificacao = null;

        Descricao = config.Agendamento.Descricao;
        Valor = config.Valor;
        ContaBancariaId = config.ContaBancariaId;
        PessoaId = config.PessoaId;
        CategoriaId = config.Agendamento.CategoriaId;
        CentroClassificacaoId = config.Agendamento.CentroClassificacaoId;
        DespesaOpcional = config.DespesaOpcional;

        foreach (var item in Parcelas.Where(w => !w.EstahPaga))
        {
            item.PessoaId = config.PessoaId;
            item.ContaBancariaId = config.ContaBancariaId;
            item.Valor = config.Valor;
            item.DespesaOpcional = config.DespesaOpcional;

            if (TipoRecorrencia != TipoRecorrencia.Semanal)
            {
                try
                {
                    item.Data = new DateTime(
                        item.Data.Year, item.Data.Month, config.Data.Day,
                        item.Data.Hour, item.Data.Minute, item.Data.Second);
                }
                catch (Exception)
                {
                    item.Data = new DateTime(
                        item.Data.Year, item.Data.Month, item.Data.Day,
                        item.Data.Hour, item.Data.Minute, item.Data.Second);
                }
            }
        }
    }
}