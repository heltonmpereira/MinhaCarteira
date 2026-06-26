using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MinhaCarteira.AppCliente.Attribute;

public class HasAtLeastOneUpperCaseLetterAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string _errorMessage;
    public HasAtLeastOneUpperCaseLetterAttribute()
    {

        _errorMessage = "A senha deve conter ao menos uma letra maiúscula.";
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        if (!context.Attributes.Any(a => a.Key == "data-val"))
            context.Attributes.Add("data-val", "true");

        context.Attributes.Add("data-val-passworduppercase", _errorMessage);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var password = value as string;

        if (!password.Any(a => char.IsUpper(a)))
            return new ValidationResult(_errorMessage);

        return ValidationResult.Success;

    }
}