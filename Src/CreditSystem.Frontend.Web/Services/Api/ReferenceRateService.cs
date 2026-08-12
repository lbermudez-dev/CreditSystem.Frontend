using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IReferenceRateService" />
public sealed class ReferenceRateService : IReferenceRateService
{
    private const string BasePath = "api/v1/reference-rates";

    private readonly IApiClient _apiClient;

    public ReferenceRateService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<ReferenceRateResponse>> GetAsync(string rateId, CancellationToken ct = default)
        => _apiClient.GetAsync<ReferenceRateResponse>($"{BasePath}/{Uri.EscapeDataString(rateId)}", ct);

    public Task<ApiResult<ReferenceRateResponse>> UpdateAsync(string rateId, UpdateReferenceRateRequest request, CancellationToken ct = default)
        => _apiClient.PutAsync<UpdateReferenceRateRequest, ReferenceRateResponse>($"{BasePath}/{Uri.EscapeDataString(rateId)}", request, ct);
}
