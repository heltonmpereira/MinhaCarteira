using System;
using Microsoft.Extensions.Logging;

namespace MinhaCarteira.AppServer.Helper;

public class DatabaseLoggerProvider : ILoggerProvider
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseLoggerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(categoryName, _serviceProvider);
    }

    public void Dispose()
    {
    }
}
