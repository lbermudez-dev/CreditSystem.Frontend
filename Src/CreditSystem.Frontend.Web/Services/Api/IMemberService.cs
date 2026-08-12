using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Cooperative Members" del swagger (base /api/v1/members).</summary>
public interface IMemberService
{
    /// <summary>POST /api/v1/members/enroll</summary>
    Task<ApiResult<EnrollMemberResponse>> EnrollAsync(EnrollMemberRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/members/{externalId}</summary>
    Task<ApiResult<MemberProfileResponse>> GetProfileAsync(Guid externalId, CancellationToken ct = default);

    /// <summary>GET /api/v1/members/{externalId}/shares</summary>
    Task<ApiResult<MemberSharesResponse>> GetSharesAsync(Guid externalId, CancellationToken ct = default);

    /// <summary>GET /api/v1/members/{externalId}/social-capital</summary>
    Task<ApiResult<SocialCapitalBalanceResponse>> GetSocialCapitalBalanceAsync(Guid externalId, CancellationToken ct = default);
}
