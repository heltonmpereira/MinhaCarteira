using System;
using System.Net.Http;
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
            baseUrlApi = baseUrlApi.Remove(baseUrlApi.Length - 1);

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<AuthorizationMessageHandler>();

        //var primaryHttpMessageHandler = new HttpClientHandler
        //{
        //    ServerCertificateCustomValidationCallback =
        //        (message, cert, chain, sslErrors) => true
        //};

        services
            .AddRefitClient<ILoginRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/login"));

        services
            .AddRefitClient<IUsuarioRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Usuario"));

        services
            .AddRefitClient<IPapelRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Papel"));

        services
            .AddRefitClient<IInstituicaoFinanceiraRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/InstituicaoFinanceira"));

        services
            .AddRefitClient<ICategoriaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Categoria"));

        services
            .AddRefitClient<ICentroClassificacaoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/CentroClassificacao"));

        services
            .AddRefitClient<IPessoaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Pessoa"));

        services
            .AddRefitClient<IContaBancariaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/ContaBancaria"));

        services
            .AddRefitClient<IMovimentoBancarioRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/MovimentoBancario"));

        services
            .AddRefitClient<IAgendamentoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Agendamento"));

        services
            .AddRefitClient<IRegraImportacaoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/RegraImportacao"));

        services
            .AddRefitClient<IImportarArquivoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/ImportarArquivo"));

        services
            .AddRefitClient<IDashboardMonitorRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/DashboardMonitor"));

        services
            .AddRefitClient<IHomeRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Home"));

        services
            .AddRefitClient<IRelatorioRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Relatorio"));

        services
            .AddRefitClient<IArquivoRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Arquivo"));

        services
            .AddRefitClient<IAuditoriaRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Auditoria"));

        services
            .AddRefitClient<ILogRefit>()
            .AddHttpMessageHandler<AuthorizationMessageHandler>()
            .ConfigureHttpClient(c => c.BaseAddress =
                new Uri($"{baseUrlApi}/Log"));

        return services;
    }
}
