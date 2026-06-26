using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.Definicao.Entidade;
using MinhaCarteira.Definicao.Interface.Repositorio;
using MinhaCarteira.Definicao.Modelo;
using Microsoft.Extensions.DependencyInjection;

namespace MinhaCarteira.AppServer.Helper;

public class DatabaseLogger : ILogger
{
    private readonly string _categoryName;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatabaseLogger(string categoryName, IServiceProvider serviceProvider)
    {
        _categoryName = categoryName;
        _serviceProvider = serviceProvider;
        _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var logRepositorio = scope.ServiceProvider.GetRequiredService<ILogRepositorio>();

                var log = new Log
                {
                    DataHora = DateTime.Now,
                    TipoLog = ConvertLogLevel(logLevel),
                    Categoria = _categoryName,
                    Mensagem = formatter(state, exception),
                    DadosSerializados = state != null ? JsonSerializer.Serialize(state) : null,
                    StackTrace = exception?.StackTrace,
                    IpUsuario = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                    Url = _httpContextAccessor.HttpContext?.Request?.Path,
                    MetodoHttp = _httpContextAccessor.HttpContext?.Request?.Method,
                    StatusCode = _httpContextAccessor.HttpContext?.Response?.StatusCode
                };

                var usuarioIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UsuarioId");
                if (usuarioIdClaim != null && Guid.TryParse(usuarioIdClaim.Value, out var usuarioId))
                {
                    log.UsuarioId = usuarioId;
                }

                var organizacaoIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("OrganizacaoId");
                if (organizacaoIdClaim != null && Guid.TryParse(organizacaoIdClaim.Value, out var organizacaoId))
                {
                    log.OrganizacaoId = organizacaoId;
                }

                await logRepositorio.Incluir(log);
            }
            catch
            {
                // Se falhar ao logar, não podemos logar o erro de logging para evitar loops
            }
        });
    }

    private static TipoLogEnum ConvertLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => TipoLogEnum.Trace,
            LogLevel.Debug => TipoLogEnum.Debug,
            LogLevel.Information => TipoLogEnum.Information,
            LogLevel.Warning => TipoLogEnum.Warning,
            LogLevel.Error => TipoLogEnum.Error,
            LogLevel.Critical => TipoLogEnum.Critical,
            _ => TipoLogEnum.Information
        };
    }
}
