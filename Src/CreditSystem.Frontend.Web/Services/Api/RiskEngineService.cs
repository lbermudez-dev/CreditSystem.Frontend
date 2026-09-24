using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IRiskEngineService" />
public sealed class RiskEngineService : IRiskEngineService
{
    private const string RiskRulesBasePath = "api/v1/risk-rules";
    private const string RiskMatricesBasePath = "api/v1/risk-matrices";
    private const string RiskEvaluationsBasePath = "api/v1/risk-evaluations";

    private readonly IApiClient _apiClient;

    public RiskEngineService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<RiskRuleResponse>>> ListRiskRulesAsync(CancellationToken ct = default)
        => _apiClient.GetAsync<List<RiskRuleResponse>>(RiskRulesBasePath, ct);

    public Task<ApiResult<RiskRuleResponse>> GetRiskRuleAsync(Guid id, CancellationToken ct = default)
        => _apiClient.GetAsync<RiskRuleResponse>($"{RiskRulesBasePath}/{id}", ct);

    public Task<ApiResult<RiskRuleResponse>> CreateRiskRuleAsync(CreateRiskRuleRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<CreateRiskRuleRequest, RiskRuleResponse>(RiskRulesBasePath, request, ct: ct);

    public Task<ApiResult<RiskRuleResponse>> UpdateRiskRuleAsync(Guid id, UpdateRiskRuleRequest request, CancellationToken ct = default)
        => _apiClient.PutAsync<UpdateRiskRuleRequest, RiskRuleResponse>($"{RiskRulesBasePath}/{id}", request, ct);

    public Task<ApiResult> DeleteRiskRuleAsync(Guid id, CancellationToken ct = default)
        => _apiClient.DeleteAsync($"{RiskRulesBasePath}/{id}", ct);

    public Task<ApiResult<List<RiskMatrixResponse>>> ListRiskMatricesAsync(CancellationToken ct = default)
        => _apiClient.GetAsync<List<RiskMatrixResponse>>(RiskMatricesBasePath, ct);

    public Task<ApiResult<RiskMatrixDetailResponse>> GetRiskMatrixAsync(Guid id, CancellationToken ct = default)
        => _apiClient.GetAsync<RiskMatrixDetailResponse>($"{RiskMatricesBasePath}/{id}", ct);

    public Task<ApiResult<RiskMatrixResponse>> CreateRiskMatrixAsync(CreateRiskMatrixRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<CreateRiskMatrixRequest, RiskMatrixResponse>(RiskMatricesBasePath, request, ct: ct);

    public Task<ApiResult<RiskMatrixResponse>> UpdateRiskMatrixAsync(Guid id, UpdateRiskMatrixRequest request, CancellationToken ct = default)
        => _apiClient.PutAsync<UpdateRiskMatrixRequest, RiskMatrixResponse>($"{RiskMatricesBasePath}/{id}", request, ct);

    public Task<ApiResult> DeleteRiskMatrixAsync(Guid id, CancellationToken ct = default)
        => _apiClient.DeleteAsync($"{RiskMatricesBasePath}/{id}", ct);

    public Task<ApiResult> ActivateRiskMatrixAsync(Guid id, CancellationToken ct = default)
        => _apiClient.PostAsync($"{RiskMatricesBasePath}/{id}/activate", ct);

    public Task<ApiResult<RiskEvaluationResponse>> TriggerRiskEvaluationAsync(TriggerRiskEvaluationRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<TriggerRiskEvaluationRequest, RiskEvaluationResponse>(RiskEvaluationsBasePath, request, ct: ct);
}
