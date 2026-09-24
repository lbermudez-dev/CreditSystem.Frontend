using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreditSystem.Frontend.Web.Models.Common;
using Microsoft.Extensions.Logging;
using ProblemDetailsModel = CreditSystem.Frontend.Web.Models.Common.ProblemDetails;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Nombre del HttpClient nombrado registrado en Program.cs.</summary>
public static class ApiClientConstants
{
    public const string HttpClientName = "CreditSystemApi";
}

/// <inheritdoc cref="IApiClient" />
public sealed class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiClient> _logger;

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ApiResult<TResponse>> GetAsync<TResponse>(string requestUri, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var response = await client.GetAsync(requestUri, ct);
            return await ReadResponseAsync<TResponse>(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult<TResponse>.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest body,
        IDictionary<string, string>? extraHeaders = null,
        CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(body, options: SerializerOptions)
            };
            AddHeaders(request, extraHeaders);

            using var response = await client.SendAsync(request, ct);
            return await ReadResponseAsync<TResponse>(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult<TResponse>.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult> PostAsync<TRequest>(
        string requestUri,
        TRequest body,
        IDictionary<string, string>? extraHeaders = null,
        CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(body, options: SerializerOptions)
            };
            AddHeaders(request, extraHeaders);

            using var response = await client.SendAsync(request, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult> PostAsync(string requestUri, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var response = await client.PostAsync(requestUri, content: null, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult<TResponse>> PostAsync<TResponse>(string requestUri, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var response = await client.PostAsync(requestUri, content: null, ct);
            return await ReadResponseAsync<TResponse>(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult<TResponse>.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
        string requestUri,
        TRequest body,
        CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var response = await client.PutAsJsonAsync(requestUri, body, SerializerOptions, ct);
            return await ReadResponseAsync<TResponse>(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult<TResponse>.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult> PutAsync<TRequest>(
        string requestUri,
        TRequest body,
        CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var response = await client.PutAsJsonAsync(requestUri, body, SerializerOptions, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult> DeleteAsync(string requestUri, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var response = await client.DeleteAsync(requestUri, ct);
            return await ReadResponseAsync(response, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult.Failure(BuildTransportError(ex, requestUri));
        }
    }

    public async Task<ApiResult<DocumentFile>> GetFileAsync(string requestUri, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient(ApiClientConstants.HttpClientName);

        try
        {
            using var response = await client.GetAsync(requestUri, ct);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<DocumentFile>.Failure(await BuildApiErrorAsync(response, ct));
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(ct);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                ?? response.Content.Headers.ContentDisposition?.FileName
                ?? "documento";

            return ApiResult<DocumentFile>.Success(new DocumentFile
            {
                Content = bytes,
                ContentType = contentType,
                FileName = fileName.Trim('"')
            });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult<DocumentFile>.Failure(BuildTransportError(ex, requestUri));
        }
    }

    private static void AddHeaders(HttpRequestMessage request, IDictionary<string, string>? headers)
    {
        if (headers is null)
        {
            return;
        }

        foreach (var (key, value) in headers)
        {
            request.Headers.TryAddWithoutValidation(key, value);
        }
    }

    private async Task<ApiResult<TResponse>> ReadResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return ApiResult<TResponse>.Success(default!);
            }

            try
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>(SerializerOptions, ct);
                return ApiResult<TResponse>.Success(data!);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializando respuesta exitosa de {Uri}", response.RequestMessage?.RequestUri);
                return ApiResult<TResponse>.Failure(new ApiError
                {
                    StatusCode = (int)response.StatusCode,
                    Message = "La respuesta del servidor no tiene el formato esperado."
                });
            }
        }

        return ApiResult<TResponse>.Failure(await BuildApiErrorAsync(response, ct));
    }

    private async Task<ApiResult> ReadResponseAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return ApiResult.Success();
        }

        return ApiResult.Failure(await BuildApiErrorAsync(response, ct));
    }

    private async Task<ApiError> BuildApiErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var statusCode = (int)response.StatusCode;

        ProblemDetailsModel? problem = null;
        try
        {
            problem = await response.Content.ReadFromJsonAsync<ProblemDetailsModel>(SerializerOptions, ct);
        }
        catch (JsonException)
        {
            // El body de error no vino en formato ProblemDetails (ej. respuestas
            // 404 sin content, como en varios endpoints del grupo Documents).
        }

        _logger.LogWarning(
            "La API respondio {StatusCode} para {Uri}: {Detail}",
            statusCode,
            response.RequestMessage?.RequestUri,
            problem?.Detail ?? problem?.Title);

        return new ApiError
        {
            StatusCode = statusCode,
            Message = statusCode switch
            {
                401 => "No autorizado. Verifique la configuracion de la ApiKey.",
                403 => "No tiene permisos para realizar esta accion.",
                404 => "El recurso solicitado no existe.",
                409 => problem?.Detail ?? "La operacion entra en conflicto con el estado actual del recurso.",
                400 or 422 => problem?.Detail ?? problem?.Title ?? "Los datos enviados no son validos.",
                >= 500 => "Ocurrio un error en el servidor. Intente nuevamente mas tarde.",
                _ => problem?.Detail ?? problem?.Title ?? "Ocurrio un error inesperado."
            },
            ValidationErrors = problem?.Errors
        };
    }

    private ApiError BuildTransportError(Exception ex, string requestUri)
    {
        _logger.LogError(ex, "Error de red/timeout llamando a {Uri}", requestUri);

        var message = ex is TaskCanceledException
            ? "La solicitud tardo demasiado en responder (timeout)."
            : "No fue posible conectarse con el servidor. Verifique su conexion.";

        return new ApiError { Message = message };
    }
}
