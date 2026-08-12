using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IPaymentService" />
public sealed class PaymentService : IPaymentService
{
    private const string BasePath = "api/v1/payments";

    private readonly IApiClient _apiClient;

    public PaymentService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<PaymentAcceptedResponse>> SubmitLoanPaymentAsync(SubmitPaymentRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<SubmitPaymentRequest, PaymentAcceptedResponse>(BasePath, request, ct: ct);

    public Task<ApiResult<RevolvingPaymentAcceptedResponse>> SubmitRevolvingPaymentAsync(SubmitRevolvingPaymentRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<SubmitRevolvingPaymentRequest, RevolvingPaymentAcceptedResponse>($"{BasePath}/revolving", request, ct: ct);

    public Task<ApiResult<PaymentStatusResponse>> GetStatusAsync(Guid paymentId, CancellationToken ct = default)
        => _apiClient.GetAsync<PaymentStatusResponse>($"{BasePath}/{paymentId}/status", ct);
}
