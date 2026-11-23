using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace MinhaCarteira.AppCliente.Refit.Middleware;

public static class ClienteRefitMiddleware
{
    public static IServiceCollection AdicionarConexoesRefit(
        this IServiceCollection services, string baseUrlApi)
    {
        if (baseUrlApi.EndsWith('/'))
            baseUrlApi = baseUrlApi[..^1];

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<AuthorizationMessageHandler>();

        //var primaryHttpMessageHandler = new HttpClientHandler
        //{
        //    ServerCertificateCustomValidationCallback =
        //        (message, cert, chain, sslErrors) => true
        //};

        services
            .AddRefitGeneratedClient<ILoginRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/login"));

        services
            .AddRefitGeneratedClient<IUsuarioRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Usuario"));

        services
            .AddRefitGeneratedClient<IPapelRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Papel"));

        services
            .AddRefitGeneratedClient<IInstituicaoFinanceiraRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/InstituicaoFinanceira"));

        services
            .AddRefitGeneratedClient<ICategoriaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Categoria"));

        services
            .AddRefitGeneratedClient<ICentroClassificacaoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/CentroClassificacao"));

        services
            .AddRefitGeneratedClient<IPessoaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Pessoa"));

        services
            .AddRefitGeneratedClient<IContaBancariaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/ContaBancaria"));

        services
            .AddRefitGeneratedClient<IMovimentoBancarioRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/MovimentoBancario"));

        services
            .AddRefitGeneratedClient<IAgendamentoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Agendamento"));

        services
            .AddRefitGeneratedClient<IRegraImportacaoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/RegraImportacao"));

        services
            .AddRefitGeneratedClient<IImportarArquivoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/ImportarArquivo"));

        services
            .AddRefitGeneratedClient<IDashboardMonitorRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/DashboardMonitor"));

        services
            .AddRefitGeneratedClient<IHomeRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Home"));

        services
            .AddRefitGeneratedClient<IRelatorioRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Relatorio"));

        services
            .AddRefitGeneratedClient<IArquivoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Arquivo"));

        services
            .AddRefitGeneratedClient<IAuditoriaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Auditoria"));

        services
            .AddRefitGeneratedClient<ILogRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Log"));

        return services;
    }
}
