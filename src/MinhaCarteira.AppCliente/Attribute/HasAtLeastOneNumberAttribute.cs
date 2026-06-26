using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MinhaCarteira.AppCliente.Attribute;

public class HasAtLeastOneNumberAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string _errorMessage;
    public HasAtLeastOneNumberAttribute()
    {

        _errorMessage = "A senha deve conter ao menos um número.";
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        if (!context.Attributes.Any(a => a.Key == "data-val"))
            context.Attributes.Add("data-val", "true");

        context.Attributes.Add("data-val-passwordnumber", _errorMessage);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var password = value as string;

        if (!password.Any(a => char.IsNumber(a)))
            return new ValidationResult(_errorMessage);


        return ValidationResult.Success;

    }
}