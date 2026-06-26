using System;

namespace MinhaCarteira.AppCliente.ViewModel;

public class TransferenciaBancariaViewModel
{
    public Guid Id { get; set; }
    public Guid MovimentoOrigemId { get; set; }
    public Guid MovimentoDestinoId { get; set; }

    public MovimentoBancarioViewModel Origem { get; set; }
    public MovimentoBancarioViewModel Destino { get; set; }
}