using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IDelinquentLoanService" />
public sealed class DelinquentLoanService : IDelinquentLoanService
{
    private const string BasePath = "api/v1/delinquent-loans";

    private readonly IApiClient _apiClient;

    public DelinquentLoanService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<DelinquentLoanResponse>>> GetAsync(int? minDaysOverdue = null, string? collectionStatus = null, CancellationToken ct = default)
    {
        var url = new QueryStringBuilder()
            .Add("minDaysOverdue", minDaysOverdue)
            .Add("collectionStatus", collectionStatus)
            .BuildUrl(BasePath);
        return _apiClient.GetAsync<List<DelinquentLoanResponse>>(url, ct);
    }

    public Task<ApiResult<DelinquentLoanResponse>> GetByIdAsync(Guid loanId, CancellationToken ct = default)
        => _apiClient.GetAsync<DelinquentLoanResponse>($"{BasePath}/{loanId}", ct);
}
