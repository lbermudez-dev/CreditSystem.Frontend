using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Grupo "Loan Contracts" del swagger (base /api/v1/loans).
/// </summary>
public interface ILoanService
{
    /// <summary>POST /api/v1/loans</summary>
    Task<ApiResult<CreateContractResponse>> CreateAsync(CreateContractCommand request, CancellationToken ct = default);

    /// <summary>POST /api/v1/loans/{id}/disburse</summary>
    Task<ApiResult<DisburseLoanResponse>> DisburseAsync(Guid loanId, DisburseLoanRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/{id}</summary>
    Task<ApiResult<LoanSummaryResponse>> GetSummaryAsync(Guid loanId, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/customer/{externalCustomerId}</summary>
    Task<ApiResult<List<LoanSummaryResponse>>> GetByCustomerAsync(Guid externalCustomerId, CancellationToken ct = default);

    /// <summary>POST /api/v1/loans/{id}/payments</summary>
    Task<ApiResult<ApplyPaymentResponse>> ApplyPaymentAsync(Guid loanId, ApplyPaymentRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/{id}/payments</summary>
    Task<ApiResult<List<PaymentHistoryResponse>>> GetPaymentHistoryAsync(Guid loanId, CancellationToken ct = default);

    /// <summary>POST /api/v1/loans/{id}/default</summary>
    Task<ApiResult<DefaultContractResponse>> MarkAsDefaultAsync(Guid loanId, DefaultContractRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/defaulted?fromDate=&amp;toDate=</summary>
    Task<ApiResult<List<DefaultedLoanResponse>>> GetDefaultedAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);

    /// <summary>POST /api/v1/loans/{id}/restructure</summary>
    Task<ApiResult<RestructureContractResponse>> RestructureAsync(Guid loanId, RestructureContractRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/{id}/restructure-history</summary>
    Task<ApiResult<List<RestructureHistoryResponse>>> GetRestructureHistoryAsync(Guid loanId, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/{id}/payoff-amount?asOfDate=</summary>
    Task<ApiResult<PayoffAmountResponse>> GetPayoffAmountAsync(Guid loanId, DateTime? asOfDate = null, CancellationToken ct = default);

    /// <summary>POST /api/v1/loans/{id}/payoff</summary>
    Task<ApiResult<PayoffContractResponse>> PayoffAsync(Guid loanId, PayoffContractRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/paid-off?fromDate=&amp;toDate=&amp;earlyPayoffOnly=</summary>
    Task<ApiResult<List<PaidOffLoanResponse>>> GetPaidOffAsync(DateTime? fromDate = null, DateTime? toDate = null, bool? earlyPayoffOnly = null, CancellationToken ct = default);
}
