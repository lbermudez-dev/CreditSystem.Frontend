using CreditSystem.Frontend.Web.Models.Common;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IAdminService" />
public sealed class AdminService : IAdminService
{
    private const string BasePath = "api/v1/admin";

    private readonly IApiClient _apiClient;

    public AdminService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult> RunInterestAccrualJobAsync(CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/jobs/interest-accrual", ct);

    public Task<ApiResult> AccrueInterestForLoanAsync(Guid loanId, CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/loans/{loanId}/accrue-interest", ct);

    public Task<ApiResult> RunPaymentMissedJobAsync(CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/jobs/payment-missed", ct);

    public Task<ApiResult> RunRevolvingInterestAccrualJobAsync(CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/jobs/revolving-interest-accrual", ct);

    public Task<ApiResult> RunStatementGenerationJobAsync(CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/jobs/statement-generation", ct);

    public Task<ApiResult> RunRevolvingPaymentMissedJobAsync(CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/jobs/revolving-payment-missed", ct);

    public Task<ApiResult> AccrueInterestForCreditLineAsync(Guid creditLineId, CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/revolving-credits/{creditLineId}/accrue-interest", ct);

    public Task<ApiResult> GenerateStatementForCreditLineAsync(Guid creditLineId, CancellationToken ct = default)
        => _apiClient.PostAsync($"{BasePath}/revolving-credits/{creditLineId}/generate-statement", ct);
}
