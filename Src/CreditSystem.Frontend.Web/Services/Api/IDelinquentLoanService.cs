using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Delinquent Loans" del swagger (base /api/v1/delinquent-loans).</summary>
public interface IDelinquentLoanService
{
    /// <summary>GET /api/v1/delinquent-loans?minDaysOverdue=&amp;collectionStatus=</summary>
    Task<ApiResult<List<DelinquentLoanResponse>>> GetAsync(int? minDaysOverdue = null, string? collectionStatus = null, CancellationToken ct = default);

    /// <summary>GET /api/v1/delinquent-loans/{id}</summary>
    Task<ApiResult<DelinquentLoanResponse>> GetByIdAsync(Guid loanId, CancellationToken ct = default);
}
