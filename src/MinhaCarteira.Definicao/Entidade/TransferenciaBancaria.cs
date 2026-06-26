using System;
using Dhani.Utilitarios.Helper;
using MinhaCarteira.Definicao.Interface.Entidade;
using MinhaCarteira.Definicao.Modelo;

namespace MinhaCarteira.Definicao.Entidade;

public class TransferenciaBancaria : IEntidade<Guid>
{
    public TransferenciaBancaria() { }
    public TransferenciaBancaria(MovimentoBancarioTransferencia dadosTransferencia)
    {
        Origem = new MovimentoBancario();
        Origem.Mapear(dadosTransferencia);

        Destino = new MovimentoBancario();
        Destino.Mapear(dadosTransferencia);

        Origem.TipoMovimento = dadosTransferencia.TipoMovimento;
        Destino.ContaBancaria = dadosTransferencia.ContaBancariaDestino;
        Destino.ContaBancariaId = dadosTransferencia.ContaBancariaDestinoId;
        Destino.TipoMovimento = dadosTransferencia.TipoMovimento == TipoMovimento.Debito
            ? TipoMovimento.Credito
            : TipoMovimento.Debito;
    }

    public Guid Id { get; set; }
    public Guid MovimentoOrigemId { get; set; }
    public Guid MovimentoDestinoId { get; set; }

    public MovimentoBancario Origem { get; set; }
    public MovimentoBancario Destino { get; set; }

    public bool Deletado { get; set; }
}