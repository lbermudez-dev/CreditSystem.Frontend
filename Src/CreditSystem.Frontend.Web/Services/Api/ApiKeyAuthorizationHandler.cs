using Microsoft.Extensions.Options;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// DelegatingHandler que agrega la ApiKey a cada request saliente hacia el
/// backend. Centralizado aqui para no repetir la logica de autenticacion
/// en cada servicio (IAuthService no aplica: no hay login/usuario, es
/// autenticacion de servicio a servicio via ApiKey estatica).
/// </summary>
public sealed class ApiKeyAuthorizationHandler : DelegatingHandler
{
    private readonly ApiSettings _settings;

    public ApiKeyAuthorizationHandler(IOptions<ApiSettings> options)
    {
        _settings = options.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_settings.ApiKey) &&
            !request.Headers.Contains(_settings.ApiKeyHeaderName))
        {
            request.Headers.Add(_settings.ApiKeyHeaderName, _settings.ApiKey);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
