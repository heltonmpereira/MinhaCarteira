using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using MinhaCarteira.AppServer;
using Xunit;

namespace MinhaCarteira.Teste.AppServer.Base;

public class TestFixture : IDisposable
{
    private readonly WebApplicationFactory<Startup> _factory;

    public TestFixture()
    {
        _factory = new MinhaCarteiraWebApplicationFactory();
        _factory.ClientOptions.BaseAddress = new Uri("http://localhost:9999");

        Client = _factory.CreateClient();
    }

    public HttpClient Client { get; }

    #region Dispose
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                Client.Dispose();
                _factory.Dispose();
            }
        }
        disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}

[CollectionDefinition("TestesDeIntegracao")]
public class TestCollection : ICollectionFixture<TestFixture>
{
}