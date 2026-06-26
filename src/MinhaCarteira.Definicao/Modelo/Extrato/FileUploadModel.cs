using System;
using MinhaCarteira.Definicao.Entidade;

namespace MinhaCarteira.Definicao.Modelo.Extrato;

public class FileUploadModel
{
    public byte[] Conteudo { get; set; }
    public string NomeArquivo { get; set; }
    public Guid ProprietarioId { get; set; }
    public Guid ContaBancariaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; }
}