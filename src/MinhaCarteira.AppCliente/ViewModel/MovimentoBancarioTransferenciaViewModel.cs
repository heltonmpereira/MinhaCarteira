using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinhaCarteira.AppCliente.ViewModel;

public class MovimentoBancarioTransferenciaViewModel : MovimentoBancarioViewModel
{
    [Required]
    [DisplayName("Conta de destino")]
    public Guid ContaBancariaDestinoId { get; set; }
    [DisplayName("Conta de destino")]
    public ContaBancariaViewModel ContaBancariaDestino { get; set; }
    private string _nomeContaBancariaDestino;
    public string NomeContaBancariaDestino
    {
        get => ContaBancariaDestino != null
            ? ContaBancariaDestino.Nome
            : _nomeContaBancariaDestino;
        set => _nomeContaBancariaDestino = value;
    }
}
