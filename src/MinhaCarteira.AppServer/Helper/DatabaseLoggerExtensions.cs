using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MinhaCarteira.AppServer.Helper;

public static class DatabaseLoggerExtensions
{
    public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();
        return builder;
    }
}
