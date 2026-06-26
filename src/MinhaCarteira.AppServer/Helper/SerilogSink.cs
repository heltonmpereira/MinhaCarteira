using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Configuration;
using Serilog.Events;
using LogEntidade = MinhaCarteira.Definicao.Entidade.Log;
using MinhaCarteira.Definicao.Modelo;
using MinhaCarteira.Modelo.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MinhaCarteira.AppServer.Helper;

public class SerilogSink : ILogEventSink
{
    private readonly IServiceProvider _serviceProvider;

    public SerilogSink(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MinhaCarteiraContext>();
                var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

                var log = new LogEntidade
                {
                    Id = Guid.NewGuid(),
                    Deletado = false,
                    DataHora = logEvent.Timestamp.UtcDateTime,
                    TipoLog = ConvertLogLevel(logEvent.Level),
                    Categoria = logEvent.Properties.TryGetValue("SourceContext", out var sourceContext)
                        ? sourceContext.ToString().Trim('"')
                        : null,
                    Mensagem = logEvent.RenderMessage(),
                    StackTrace = logEvent.Exception?.ToString(),
                    IpUsuario = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                    Url = httpContextAccessor.HttpContext?.Request?.Path.ToString(),
                    MetodoHttp = httpContextAccessor.HttpContext?.Request?.Method,
                    StatusCode = httpContextAccessor.HttpContext?.Response?.StatusCode
                };

                // Mapear propriedades adicionais
                var properties = logEvent.Properties
                    .Where(p => p.Key != "SourceContext")
                    .ToDictionary(p => p.Key, p => p.Value.ToString());

                if (properties.Any())
                {
                    log.DadosSerializados = JsonSerializer.Serialize(properties);
                }

                // Obter dados do usuário do contexto
                var usuarioIdClaim = httpContextAccessor.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == "UsuarioId");
                if (usuarioIdClaim != null && Guid.TryParse(usuarioIdClaim.Value, out var usuarioId))
                {
                    log.UsuarioId = usuarioId;
                }

                var organizacaoIdClaim = httpContextAccessor.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == "OrganizacaoId");
                if (organizacaoIdClaim != null && Guid.TryParse(organizacaoIdClaim.Value, out var organizacaoId))
                {
                    log.OrganizacaoId = organizacaoId;
                }

                context.Logs.Add(log);
                await context.SaveChangesAsync();
            }
            catch
            {
                // Não logar erro de logging para evitar loops
            }
        });
    }

    private static TipoLogEnum ConvertLogLevel(LogEventLevel logLevel)
    {
        return logLevel switch
        {
            LogEventLevel.Verbose => TipoLogEnum.Trace,
            LogEventLevel.Debug => TipoLogEnum.Debug,
            LogEventLevel.Information => TipoLogEnum.Information,
            LogEventLevel.Warning => TipoLogEnum.Warning,
            LogEventLevel.Error => TipoLogEnum.Error,
            LogEventLevel.Fatal => TipoLogEnum.Critical,
            _ => TipoLogEnum.Information
        };
    }
}

public static class SerilogSinkExtensions
{
    public static LoggerConfiguration WriteToDatabase(this LoggerSinkConfiguration sinkConfiguration,
        IServiceProvider serviceProvider)
    {
        return sinkConfiguration.Sink(new SerilogSink(serviceProvider));
    }
}