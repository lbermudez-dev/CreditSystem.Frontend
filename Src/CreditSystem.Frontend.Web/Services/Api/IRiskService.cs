using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Risk Classification" del swagger (base /api/v1/loans/risk-summary).</summary>
public interface IRiskService
{
    /// <summary>GET /api/v1/loans/risk-summary</summary>
    Task<ApiResult<RiskSummaryResponse>> GetSummaryAsync(CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/risk-summary/{category}</summary>
    Task<ApiResult<List<LoanRiskDetailRow>>> GetCategoryDetailAsync(string category, CancellationToken ct = default);

    /// <summary>PUT /api/v1/loans/{loanId}/risk-category</summary>
    Task<ApiResult> ManualReclassifyAsync(Guid loanId, ManualReclassifyRequest request, CancellationToken ct = default);
}
