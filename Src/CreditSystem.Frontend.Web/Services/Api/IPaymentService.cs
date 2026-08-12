using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Async Payments" del swagger (base /api/v1/payments).</summary>
public interface IPaymentService
{
    /// <summary>POST /api/v1/payments</summary>
    Task<ApiResult<PaymentAcceptedResponse>> SubmitLoanPaymentAsync(SubmitPaymentRequest request, CancellationToken ct = default);

    /// <summary>POST /api/v1/payments/revolving</summary>
    Task<ApiResult<RevolvingPaymentAcceptedResponse>> SubmitRevolvingPaymentAsync(SubmitRevolvingPaymentRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/payments/{paymentId}/status</summary>
    Task<ApiResult<PaymentStatusResponse>> GetStatusAsync(Guid paymentId, CancellationToken ct = default);
}
