using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using MinhaCarteira.AppServer;
using MinhaCarteira.Teste.AppServer.Helper;

namespace MinhaCarteira.Teste.AppServer.Base;

public class MinhaCarteiraWebApplicationFactory : WebApplicationFactory<Startup>
{
    public MinhaCarteiraWebApplicationFactory()
    {
        LocalDb.ApagarECriarLocalDb("MinhaCarteiraTesteDB");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            cfg.AddJsonFile("appSettingsTest.json", false);
        });
    }
}