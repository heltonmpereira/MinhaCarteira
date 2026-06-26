using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MinhaCarteira.AppCliente.Helper;

namespace MinhaCarteira.AppCliente.Refit.Middleware;

public class AuthorizationMessageHandler(IHttpContextAccessor httpContext) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!httpContext.HttpContext.Request.Cookies.ContainsKey(Constante.NOME_COOKIE_BEARER))
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            httpContext.HttpContext.Request.Cookies[Constante.NOME_COOKIE_BEARER]);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}