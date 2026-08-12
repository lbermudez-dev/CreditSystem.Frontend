using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="ILoanService" />
public sealed class LoanService : ILoanService
{
    private const string BasePath = "api/v1/loans";

    private readonly IApiClient _apiClient;

    public LoanService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<CreateContractResponse>> CreateAsync(CreateContractCommand request, CancellationToken ct = default)
        => _apiClient.PostAsync<CreateContractCommand, CreateContractResponse>(BasePath, request, ct: ct);

    public Task<ApiResult<DisburseLoanResponse>> DisburseAsync(Guid loanId, DisburseLoanRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<DisburseLoanRequest, DisburseLoanResponse>($"{BasePath}/{loanId}/disburse", request, ct: ct);

    public Task<ApiResult<LoanSummaryResponse>> GetSummaryAsync(Guid loanId, CancellationToken ct = default)
        => _apiClient.GetAsync<LoanSummaryResponse>($"{BasePath}/{loanId}", ct);

    public Task<ApiResult<List<LoanSummaryResponse>>> GetByCustomerAsync(Guid externalCustomerId, CancellationToken ct = default)
        => _apiClient.GetAsync<List<LoanSummaryResponse>>($"{BasePath}/customer/{externalCustomerId}", ct);

    public Task<ApiResult<ApplyPaymentResponse>> ApplyPaymentAsync(Guid loanId, ApplyPaymentRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<ApplyPaymentRequest, ApplyPaymentResponse>($"{BasePath}/{loanId}/payments", request, ct: ct);

    public Task<ApiResult<List<PaymentHistoryResponse>>> GetPaymentHistoryAsync(Guid loanId, CancellationToken ct = default)
        => _apiClient.GetAsync<List<PaymentHistoryResponse>>($"{BasePath}/{loanId}/payments", ct);

    public Task<ApiResult<DefaultContractResponse>> MarkAsDefaultAsync(Guid loanId, DefaultContractRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<DefaultContractRequest, DefaultContractResponse>($"{BasePath}/{loanId}/default", request, ct: ct);

    public Task<ApiResult<List<DefaultedLoanResponse>>> GetDefaultedAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default)
    {
        var url = new QueryStringBuilder().Add("fromDate", fromDate).Add("toDate", toDate).BuildUrl($"{BasePath}/defaulted");
        return _apiClient.GetAsync<List<DefaultedLoanResponse>>(url, ct);
    }

    public Task<ApiResult<RestructureContractResponse>> RestructureAsync(Guid loanId, RestructureContractRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<RestructureContractRequest, RestructureContractResponse>($"{BasePath}/{loanId}/restructure", request, ct: ct);

    public Task<ApiResult<List<RestructureHistoryResponse>>> GetRestructureHistoryAsync(Guid loanId, CancellationToken ct = default)
        => _apiClient.GetAsync<List<RestructureHistoryResponse>>($"{BasePath}/{loanId}/restructure-history", ct);

    public Task<ApiResult<PayoffAmountResponse>> GetPayoffAmountAsync(Guid loanId, DateTime? asOfDate = null, CancellationToken ct = default)
    {
        var url = new QueryStringBuilder().Add("asOfDate", asOfDate).BuildUrl($"{BasePath}/{loanId}/payoff-amount");
        return _apiClient.GetAsync<PayoffAmountResponse>(url, ct);
    }

    public Task<ApiResult<PayoffContractResponse>> PayoffAsync(Guid loanId, PayoffContractRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<PayoffContractRequest, PayoffContractResponse>($"{BasePath}/{loanId}/payoff", request, ct: ct);

    public Task<ApiResult<List<PaidOffLoanResponse>>> GetPaidOffAsync(DateTime? fromDate = null, DateTime? toDate = null, bool? earlyPayoffOnly = null, CancellationToken ct = default)
    {
        var url = new QueryStringBuilder()
            .Add("fromDate", fromDate)
            .Add("toDate", toDate)
            .Add("earlyPayoffOnly", earlyPayoffOnly)
            .BuildUrl($"{BasePath}/paid-off");
        return _apiClient.GetAsync<List<PaidOffLoanResponse>>(url, ct);
    }
}
