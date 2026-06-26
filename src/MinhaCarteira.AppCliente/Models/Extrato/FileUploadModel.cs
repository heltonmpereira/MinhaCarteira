using System;
using MinhaCarteira.AppCliente.ViewModel;

namespace MinhaCarteira.AppCliente.Models.Extrato;

public class FileUploadModel
{
    public byte[] Conteudo { get; set; }
    public string NomeArquivo { get; set; }
    public Guid ProprietarioId { get; set; }
    public Guid ContaBancariaId { get; set; }
    public ContaBancariaViewModel ContaBancaria { get; set; }
}