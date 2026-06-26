using System;
using MinhaCarteira.Definicao.Entidade;

namespace MinhaCarteira.Definicao.Modelo;

public class MovimentoBancarioTransferencia : MovimentoBancario
{
    public Guid ContaBancariaDestinoId { get; set; }
    public ContaBancaria ContaBancariaDestino { get; set; }
}