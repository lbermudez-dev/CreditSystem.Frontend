using CreditSystem.Frontend.Web.Models.Common;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IDocumentService" />
public sealed class DocumentService : IDocumentService
{
    private const string LoansBasePath = "api/v1/loans";

    private readonly IApiClient _apiClient;

    public DocumentService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<DocumentFile>> GetAmortizationTableAsync(Guid loanId, string format, CancellationToken ct = default)
    {
        var url = new QueryStringBuilder().Add("format", format).BuildUrl($"{LoansBasePath}/{loanId}/documents/amortization-table");
        return _apiClient.GetFileAsync(url, ct);
    }

    public Task<ApiResult<DocumentFile>> GetBalanceLetterAsync(Guid loanId, CancellationToken ct = default)
        => _apiClient.GetFileAsync($"{LoansBasePath}/{loanId}/documents/balance-letter", ct);

    public Task<ApiResult<DocumentFile>> GetPaymentReceiptAsync(Guid loanId, Guid paymentId, CancellationToken ct = default)
        => _apiClient.GetFileAsync($"{LoansBasePath}/{loanId}/documents/payment-receipt/{paymentId}", ct);
}
