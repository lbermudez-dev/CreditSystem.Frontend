using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Loan Guarantees" del swagger (base /api/v1/loans/{loanContractId}/guarantees).</summary>
public interface IGuaranteeService
{
    /// <summary>GET /api/v1/loans/{loanContractId}/guarantees</summary>
    Task<ApiResult<List<GuaranteeResponse>>> GetByContractAsync(Guid loanContractId, CancellationToken ct = default);

    /// <summary>POST /api/v1/loans/{loanContractId}/guarantees</summary>
    Task<ApiResult> RegisterAsync(Guid loanContractId, CreateGuaranteeRequest request, CancellationToken ct = default);

    /// <summary>PUT /api/v1/loans/{loanContractId}/guarantees/{guaranteeId}/status</summary>
    Task<ApiResult> UpdateStatusAsync(Guid loanContractId, Guid guaranteeId, UpdateGuaranteeStatusRequest request, CancellationToken ct = default);
}
