using CreditSystem.Frontend.Web.Models.Common;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Cliente HTTP generico y centralizado. Todos los servicios de dominio
/// (ILoanService, IRevolvingCreditService, etc.) delegan aqui la
/// serializacion/deserializacion y la interpretacion de errores, para no
/// duplicar logica HTTP en cada uno.
/// </summary>
public interface IApiClient
{
    Task<ApiResult<TResponse>> GetAsync<TResponse>(string requestUri, CancellationToken ct = default);

    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest body,
        IDictionary<string, string>? extraHeaders = null,
        CancellationToken ct = default);

    Task<ApiResult> PostAsync<TRequest>(
        string requestUri,
        TRequest body,
        IDictionary<string, string>? extraHeaders = null,
        CancellationToken ct = default);

    Task<ApiResult> PostAsync(string requestUri, CancellationToken ct = default);

    /// <summary>Para endpoints POST sin request body pero con response tipada (ej. activate).</summary>
    Task<ApiResult<TResponse>> PostAsync<TResponse>(string requestUri, CancellationToken ct = default);

    Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
        string requestUri,
        TRequest body,
        CancellationToken ct = default);

    Task<ApiResult> PutAsync<TRequest>(
        string requestUri,
        TRequest body,
        CancellationToken ct = default);

    /// <summary>
    /// Para el grupo "Documents" del swagger (application/pdf,
    /// application/vnd.openxmlformats-...sheet) — la respuesta no es JSON.
    /// </summary>
    Task<ApiResult<DocumentFile>> GetFileAsync(string requestUri, CancellationToken ct = default);
}
