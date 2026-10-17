using System;

namespace MinhaCarteira.Definicao.Modelo;

public class NotificacaoModel
{
    public Guid ProprietarioId { get; set; }
    public Guid ContaBancariaId { get; set; }
    public string Texto { get; set; }
}
