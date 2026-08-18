using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IProjectionAdminService" />
public sealed class ProjectionAdminService : IProjectionAdminService
{
    private const string BasePath = "api/v1/admin/projection";

    private readonly IApiClient _apiClient;

    public ProjectionAdminService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<ProjectionFailureResponse>>> GetFailuresAsync(bool resolved = false, CancellationToken ct = default)
    {
        var url = new QueryStringBuilder().Add("resolved", resolved).BuildUrl($"{BasePath}/failures");
        return _apiClient.GetAsync<List<ProjectionFailureResponse>>(url, ct);
    }

    public Task<ApiResult<ResolveProjectionFailureResponse>> ResolveFailureAsync(Guid failureId, CancellationToken ct = default)
        => _apiClient.PostAsync<ResolveProjectionFailureResponse>($"{BasePath}/failures/{failureId}/resolve", ct);

    public Task<ApiResult<RebuildProjectionsResponse>> RebuildAsync(CancellationToken ct = default)
        => _apiClient.PostAsync<RebuildProjectionsResponse>($"{BasePath}/rebuild", ct);
}
