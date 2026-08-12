using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Revolving Credit" del swagger (base /api/v1/revolving-credits).</summary>
public interface IRevolvingCreditService
{
    /// <summary>POST /api/v1/revolving-credits</summary>
    Task<ApiResult<CreateCreditLineResponse>> CreateAsync(CreateCreditLineRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/revolving-credits/{id}</summary>
    Task<ApiResult<RevolvingCreditSummaryResponse>> GetSummaryAsync(Guid creditLineId, CancellationToken ct = default);

    /// <summary>GET /api/v1/revolving-credits/customer/{customerId}</summary>
    Task<ApiResult<List<RevolvingCreditSummaryResponse>>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);

    /// <summary>POST /api/v1/revolving-credits/{id}/activate</summary>
    Task<ApiResult<ActivateCreditLineResponse>> ActivateAsync(Guid creditLineId, CancellationToken ct = default);

    /// <summary>POST /api/v1/revolving-credits/{id}/draw</summary>
    Task<ApiResult<DrawFundsResponse>> DrawFundsAsync(Guid creditLineId, DrawFundsRequest request, CancellationToken ct = default);

    /// <summary>POST /api/v1/revolving-credits/{id}/payments</summary>
    Task<ApiResult<ApplyRevolvingPaymentResponse>> ApplyPaymentAsync(Guid creditLineId, ApplyRevolvingPaymentRequest request, CancellationToken ct = default);

    /// <summary>POST /api/v1/revolving-credits/{id}/freeze</summary>
    Task<ApiResult<FreezeCreditLineResponse>> FreezeAsync(Guid creditLineId, FreezeCreditLineRequest request, CancellationToken ct = default);

    /// <summary>POST /api/v1/revolving-credits/{id}/unfreeze</summary>
    Task<ApiResult<UnfreezeCreditLineResponse>> UnfreezeAsync(Guid creditLineId, UnfreezeCreditLineRequest request, CancellationToken ct = default);

    /// <summary>POST /api/v1/revolving-credits/{id}/change-limit</summary>
    Task<ApiResult<ChangeCreditLimitResponse>> ChangeCreditLimitAsync(Guid creditLineId, ChangeCreditLimitRequest request, CancellationToken ct = default);

    /// <summary>POST /api/v1/revolving-credits/{id}/close</summary>
    Task<ApiResult<CloseCreditLineResponse>> CloseAsync(Guid creditLineId, CloseCreditLineRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/revolving-credits/{id}/transactions?limit=</summary>
    Task<ApiResult<List<RevolvingTransactionResponse>>> GetTransactionsAsync(Guid creditLineId, int? limit = null, CancellationToken ct = default);

    /// <summary>GET /api/v1/revolving-credits/{id}/statements</summary>
    Task<ApiResult<List<RevolvingStatementResponse>>> GetStatementsAsync(Guid creditLineId, CancellationToken ct = default);
}
