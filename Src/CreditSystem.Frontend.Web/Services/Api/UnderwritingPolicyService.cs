using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IUnderwritingPolicyService" />
public sealed class UnderwritingPolicyService : IUnderwritingPolicyService
{
    private const string BasePath = "api/v1/underwriting-policies";

    private readonly IApiClient _apiClient;

    public UnderwritingPolicyService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<UnderwritingPolicyResponse>>> GetAllAsync(CancellationToken ct = default)
        => _apiClient.GetAsync<List<UnderwritingPolicyResponse>>(BasePath, ct);
}
