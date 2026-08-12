using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IRevolvingCreditService" />
public sealed class RevolvingCreditService : IRevolvingCreditService
{
    private const string BasePath = "api/v1/revolving-credits";

    private readonly IApiClient _apiClient;

    public RevolvingCreditService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<CreateCreditLineResponse>> CreateAsync(CreateCreditLineRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<CreateCreditLineRequest, CreateCreditLineResponse>(BasePath, request, ct: ct);

    public Task<ApiResult<RevolvingCreditSummaryResponse>> GetSummaryAsync(Guid creditLineId, CancellationToken ct = default)
        => _apiClient.GetAsync<RevolvingCreditSummaryResponse>($"{BasePath}/{creditLineId}", ct);

    public Task<ApiResult<List<RevolvingCreditSummaryResponse>>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default)
        => _apiClient.GetAsync<List<RevolvingCreditSummaryResponse>>($"{BasePath}/customer/{customerId}", ct);

    public Task<ApiResult<ActivateCreditLineResponse>> ActivateAsync(Guid creditLineId, CancellationToken ct = default)
        => _apiClient.PostAsync<ActivateCreditLineResponse>($"{BasePath}/{creditLineId}/activate", ct);

    public Task<ApiResult<DrawFundsResponse>> DrawFundsAsync(Guid creditLineId, DrawFundsRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<DrawFundsRequest, DrawFundsResponse>($"{BasePath}/{creditLineId}/draw", request, ct: ct);

    public Task<ApiResult<ApplyRevolvingPaymentResponse>> ApplyPaymentAsync(Guid creditLineId, ApplyRevolvingPaymentRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<ApplyRevolvingPaymentRequest, ApplyRevolvingPaymentResponse>($"{BasePath}/{creditLineId}/payments", request, ct: ct);

    public Task<ApiResult<FreezeCreditLineResponse>> FreezeAsync(Guid creditLineId, FreezeCreditLineRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<FreezeCreditLineRequest, FreezeCreditLineResponse>($"{BasePath}/{creditLineId}/freeze", request, ct: ct);

    public Task<ApiResult<UnfreezeCreditLineResponse>> UnfreezeAsync(Guid creditLineId, UnfreezeCreditLineRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<UnfreezeCreditLineRequest, UnfreezeCreditLineResponse>($"{BasePath}/{creditLineId}/unfreeze", request, ct: ct);

    public Task<ApiResult<ChangeCreditLimitResponse>> ChangeCreditLimitAsync(Guid creditLineId, ChangeCreditLimitRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<ChangeCreditLimitRequest, ChangeCreditLimitResponse>($"{BasePath}/{creditLineId}/change-limit", request, ct: ct);

    public Task<ApiResult<CloseCreditLineResponse>> CloseAsync(Guid creditLineId, CloseCreditLineRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<CloseCreditLineRequest, CloseCreditLineResponse>($"{BasePath}/{creditLineId}/close", request, ct: ct);

    public Task<ApiResult<List<RevolvingTransactionResponse>>> GetTransactionsAsync(Guid creditLineId, int? limit = null, CancellationToken ct = default)
    {
        var url = new QueryStringBuilder().Add("limit", limit).BuildUrl($"{BasePath}/{creditLineId}/transactions");
        return _apiClient.GetAsync<List<RevolvingTransactionResponse>>(url, ct);
    }

    public Task<ApiResult<List<RevolvingStatementResponse>>> GetStatementsAsync(Guid creditLineId, CancellationToken ct = default)
        => _apiClient.GetAsync<List<RevolvingStatementResponse>>($"{BasePath}/{creditLineId}/statements", ct);
}
