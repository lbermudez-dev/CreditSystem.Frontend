using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IRiskService" />
public sealed class RiskService : IRiskService
{
    private const string LoansBasePath = "api/v1/loans";

    private readonly IApiClient _apiClient;

    public RiskService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<RiskSummaryResponse>> GetSummaryAsync(CancellationToken ct = default)
        => _apiClient.GetAsync<RiskSummaryResponse>($"{LoansBasePath}/risk-summary", ct);

    public Task<ApiResult<List<LoanRiskDetailRow>>> GetCategoryDetailAsync(string category, CancellationToken ct = default)
        => _apiClient.GetAsync<List<LoanRiskDetailRow>>($"{LoansBasePath}/risk-summary/{Uri.EscapeDataString(category)}", ct);

    public Task<ApiResult> ManualReclassifyAsync(Guid loanId, ManualReclassifyRequest request, CancellationToken ct = default)
        => _apiClient.PutAsync($"{LoansBasePath}/{loanId}/risk-category", request, ct);
}
