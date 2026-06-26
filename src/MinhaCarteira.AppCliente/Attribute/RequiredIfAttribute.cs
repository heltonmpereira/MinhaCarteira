using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MinhaCarteira.AppCliente.Attribute;

public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
{
    private string PropertyName { get; set; }
    private object DesiredValue { get; set; }
    private readonly string _errorMessage;

    public RequiredIfAttribute(string propertyName, object desiredvalue)
    {
        PropertyName = propertyName;
        DesiredValue = desiredvalue;
        _errorMessage = ErrorMessage ?? "Esse campo deve ser preenchido ou selecionado.";
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        if (!context.Attributes.Any(a => a.Key == "data-val"))
            context.Attributes.Add("data-val", "true");

        context.Attributes.Add("data-val-requiredif", _errorMessage);
        context.Attributes.Add("data-val-propertyname", PropertyName);
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var valor = value as string;

        var instance = context.ObjectInstance;
        Type type = instance.GetType();
        var proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);

        if (proprtyvalue.ToString() == DesiredValue.ToString() && string.IsNullOrWhiteSpace(valor))
            return new ValidationResult(_errorMessage);

        return ValidationResult.Success;

    }

}