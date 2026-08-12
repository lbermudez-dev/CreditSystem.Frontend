using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IGuaranteeService" />
public sealed class GuaranteeService : IGuaranteeService
{
    private const string LoansBasePath = "api/v1/loans";

    private readonly IApiClient _apiClient;

    public GuaranteeService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<GuaranteeResponse>>> GetByContractAsync(Guid loanContractId, CancellationToken ct = default)
        => _apiClient.GetAsync<List<GuaranteeResponse>>($"{LoansBasePath}/{loanContractId}/guarantees", ct);

    public Task<ApiResult> RegisterAsync(Guid loanContractId, CreateGuaranteeRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync($"{LoansBasePath}/{loanContractId}/guarantees", request, ct: ct);

    public Task<ApiResult> UpdateStatusAsync(Guid loanContractId, Guid guaranteeId, UpdateGuaranteeStatusRequest request, CancellationToken ct = default)
        => _apiClient.PutAsync($"{LoansBasePath}/{loanContractId}/guarantees/{guaranteeId}/status", request, ct);
}
