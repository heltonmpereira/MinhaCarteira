using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MinhaCarteira.AppCliente.Attribute.Base;

public class AttributesBase : ValidationAttribute
{
    private readonly IConfigurationRoot _config;

    public AttributesBase()
    {
        var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        IConfigurationBuilder builder = new ConfigurationBuilder();
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        var jsonPathEnvironment = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{ambiente}.json");

        builder.AddJsonFile(jsonPath, false, true);
        builder.AddJsonFile(jsonPathEnvironment, true, true);

        _config = builder.Build();
    }

    protected T CarregarValorConfiguracao<T>(
        string secao,
        string chave,
        T valorPadrao)
    {
        //if (secao == null) secao = "DefinicaoArquivos";
        var cfgSecao = _config?.GetSection(secao);
        if (cfgSecao == null)
            return valorPadrao;

        var valor = cfgSecao[chave];
        if (valor == null)
            return valorPadrao;

        try
        {
            return (T)Convert.ChangeType(valor, typeof(T));
        }
        catch
        {
            return valorPadrao;
        }
    }
}
