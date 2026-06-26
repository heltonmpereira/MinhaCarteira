using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Attribute.Base;

namespace MinhaCarteira.AppCliente.Attribute;

public class MaxFileSizeAttribute : AttributesBase
{
    private readonly int _maxFileSize;
    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }
    public MaxFileSizeAttribute(string chave)
    {
        _maxFileSize = CarregarValorConfiguracao("DefinicaoArquivos", chave, 0);
    }

    protected override ValidationResult IsValid(
    object value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
            return ValidationResult.Success;

        return file.Length > _maxFileSize
            ? new ValidationResult(GetErrorMessage())
            : ValidationResult.Success;
    }

    public string GetErrorMessage()
    {
        return $"Tamanho maximo permitido é de {_maxFileSize} bytes.";
    }
}
