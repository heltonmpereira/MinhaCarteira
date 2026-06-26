
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using MinhaCarteira.AppServer.Helper;

namespace MinhaCarteira.AppServer;

public class Program
{
    public static void Main(string[] args)
    {
        // Configurar Serilog primeiro para capturar erros de inicialização
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        // Criar host temporário para obter o ServiceProvider
        var tempHost = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .Build();

        // Configurar Serilog com o ServiceProvider
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.WriteToDatabase(tempHost.Services)
            .CreateLogger();

        try
        {
            Log.Information("Iniciando aplicação");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Aplicação encerrada inesperadamente");
        }
        finally
        {
            Log.CloseAndFlush();
            tempHost.Dispose();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console()
                    .WriteTo.WriteToDatabase(services);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
