using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IMemberService" />
public sealed class MemberService : IMemberService
{
    private const string BasePath = "api/v1/members";

    private readonly IApiClient _apiClient;

    public MemberService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<EnrollMemberResponse>> EnrollAsync(EnrollMemberRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<EnrollMemberRequest, EnrollMemberResponse>($"{BasePath}/enroll", request, ct: ct);

    public Task<ApiResult<MemberProfileResponse>> GetProfileAsync(Guid externalId, CancellationToken ct = default)
        => _apiClient.GetAsync<MemberProfileResponse>($"{BasePath}/{externalId}", ct);

    public Task<ApiResult<MemberSharesResponse>> GetSharesAsync(Guid externalId, CancellationToken ct = default)
        => _apiClient.GetAsync<MemberSharesResponse>($"{BasePath}/{externalId}/shares", ct);

    public Task<ApiResult<SocialCapitalBalanceResponse>> GetSocialCapitalBalanceAsync(Guid externalId, CancellationToken ct = default)
        => _apiClient.GetAsync<SocialCapitalBalanceResponse>($"{BasePath}/{externalId}/social-capital", ct);
}
