using System.Text;
using CreditSystem.Frontend.Web.Models.Common;

namespace CreditSystem.Frontend.Web.Services.Api.Dummy;

/// <summary>
/// Implementacion de <see cref="IApiClient"/> que NO llama a ningun
/// backend: responde con datos generados por <see cref="DummyDataFactory"/>.
/// Se activa por configuracion ("DummyData:Enabled" en appsettings, ver
/// Program.cs) y permite maquetar/ajustar todas las pantallas sin depender
/// de que el API real este disponible. Ninguna pagina ni servicio de
/// dominio (ILoanService, IMemberService, etc.) sabe que esta "dummy":
/// todos dependen unicamente de IApiClient.
///
/// Simula una pequena latencia de red (150-400ms) para que los estados de
/// carga (spinners, skeletons) tambien se puedan ajustar visualmente.
/// </summary>
public sealed class DummyApiClient : IApiClient
{
    private static readonly Random DelayRng = new();

    public async Task<ApiResult<TResponse>> GetAsync<TResponse>(string requestUri, CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult<TResponse>.Success(DummyDataFactory.Create<TResponse>(requestUri));
    }

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest body,
        IDictionary<string, string>? extraHeaders = null,
        CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult<TResponse>.Success(DummyDataFactory.Create<TResponse>(requestUri));
    }

    public async Task<ApiResult> PostAsync<TRequest>(
        string requestUri,
        TRequest body,
        IDictionary<string, string>? extraHeaders = null,
        CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult.Success();
    }

    public async Task<ApiResult> PostAsync(string requestUri, CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult.Success();
    }

    public async Task<ApiResult<TResponse>> PostAsync<TResponse>(string requestUri, CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult<TResponse>.Success(DummyDataFactory.Create<TResponse>(requestUri));
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
        string requestUri,
        TRequest body,
        CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult<TResponse>.Success(DummyDataFactory.Create<TResponse>(requestUri));
    }

    public async Task<ApiResult> PutAsync<TRequest>(string requestUri, TRequest body, CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult.Success();
    }

    public async Task<ApiResult> DeleteAsync(string requestUri, CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);
        return ApiResult.Success();
    }

    public async Task<ApiResult<DocumentFile>> GetFileAsync(string requestUri, CancellationToken ct = default)
    {
        await SimulateLatencyAsync(ct);

        var isSpreadsheet = requestUri.Contains("format=xlsx", StringComparison.OrdinalIgnoreCase);
        var fileName = requestUri switch
        {
            _ when requestUri.Contains("amortization-table", StringComparison.OrdinalIgnoreCase) =>
                isSpreadsheet ? "tabla-amortizacion-demo.xlsx" : "tabla-amortizacion-demo.pdf",
            _ when requestUri.Contains("balance-letter", StringComparison.OrdinalIgnoreCase) => "carta-saldo-demo.pdf",
            _ when requestUri.Contains("payment-receipt", StringComparison.OrdinalIgnoreCase) => "comprobante-pago-demo.pdf",
            _ => "documento-demo.pdf"
        };

        var content = Encoding.UTF8.GetBytes($"Documento de ejemplo generado en modo demo.\nSolicitud: {requestUri}");

        return ApiResult<DocumentFile>.Success(new DocumentFile
        {
            Content = content,
            ContentType = isSpreadsheet ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf",
            FileName = fileName
        });
    }

    private static Task SimulateLatencyAsync(CancellationToken ct)
    {
        int delayMs;
        lock (DelayRng)
        {
            delayMs = DelayRng.Next(150, 400);
        }

        return Task.Delay(delayMs, ct);
    }
}
