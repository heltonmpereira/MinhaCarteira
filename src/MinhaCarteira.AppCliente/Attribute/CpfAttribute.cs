using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MinhaCarteira.AppCliente.Attribute;

public partial class CpfAttribute : RequiredAttribute, IClientModelValidator
{
    [GeneratedRegex("[^0-9]")]
    private static partial Regex NotNaturalPattern();
    [GeneratedRegex("0*[1-9][0-9]*")]
    private static partial Regex NaturalPattern();

    private readonly string _errorMessage;
    public CpfAttribute()
    {
        _errorMessage = ErrorMessage ?? "Cpf inválido.";
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        if (context.Attributes.All(a => a.Key != "data-val"))
            context.Attributes.Add("data-val", "true");

        context.Attributes.Add("data-val-validacpf", _errorMessage);

    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var valor = value as string;

        if (Validar(valor))
            return ValidationResult.Success;

        return new ValidationResult(_errorMessage);
    }

    private static bool Validar(string valor)
    {
        if (string.IsNullOrEmpty(valor))
            return false;
        //valida se o número existe
        //var objNotNaturalPattern = new Regex("[^0-9]");
        //var objNaturalPattern = new Regex("0*[1-9][0-9]*");

        if (NotNaturalPattern().IsMatch(valor) && !NaturalPattern().IsMatch(valor))
            return false;

        valor = valor.Replace("-", "").Replace(".", "");

        if (valor.Distinct().Count() == 1)
            return false;

        var dv = valor.Substring(9, 2);
        valor = valor.Remove(9);

        string digito = CalcularDigito(valor, 1);
        digito += CalcularDigito(valor + digito, 0);

        return dv == digito;
    }

    private static string CalcularDigito(string cpf, int digito)
    {
        int soma = 0;
        for (int i = 0; i < cpf.Length; i++)
        {
            int numero = int.Parse(cpf.Substring(i, 1));
            soma += (i + digito) * numero;
        }
        return (soma % 11 == 10 ? 0 : soma % 11).ToString();
    }

}