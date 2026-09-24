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

    public Task<ApiResult<List<RiskMatrixResponse>>> ListRiskMatricesAsync(string? status = null, CancellationToken ct = default)
    {
        var uri = string.IsNullOrEmpty(status)
            ? RiskMatricesBasePath
            : $"{RiskMatricesBasePath}?status={Uri.EscapeDataString(status)}";
        return _apiClient.GetAsync<List<RiskMatrixResponse>>(uri, ct);
    }

    public Task<ApiResult<RiskMatrixDetailResponse>> GetRiskMatrixAsync(Guid id, CancellationToken ct = default)
        => _apiClient.GetAsync<RiskMatrixDetailResponse>($"{RiskMatricesBasePath}/{id}", ct);

    public Task<ApiResult<RiskMatrixResponse>> CreateRiskMatrixAsync(CreateRiskMatrixRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<CreateRiskMatrixRequest, RiskMatrixResponse>(RiskMatricesBasePath, request, ct: ct);

    // Request null -> body con todos los campos null (JsonSerializerDefaults.Web los
    // escribe como null explicito); el backend los trata como "omitido" y copia del origen.
    public Task<ApiResult<RiskMatrixDetailResponse>> CreateRiskMatrixVersionAsync(Guid id, CreateRiskMatrixVersionRequest? request = null, CancellationToken ct = default)
        => _apiClient.PostAsync<CreateRiskMatrixVersionRequest, RiskMatrixDetailResponse>(
            $"{RiskMatricesBasePath}/{id}/new-version", request ?? new CreateRiskMatrixVersionRequest(), ct: ct);

    public Task<ApiResult> DeleteRiskMatrixAsync(Guid id, CancellationToken ct = default)
        => _apiClient.DeleteAsync($"{RiskMatricesBasePath}/{id}", ct);

    public Task<ApiResult> ActivateRiskMatrixAsync(Guid id, CancellationToken ct = default)
        => _apiClient.PostAsync($"{RiskMatricesBasePath}/{id}/activate", ct);

    public Task<ApiResult<RiskEvaluationResponse>> TriggerRiskEvaluationAsync(TriggerRiskEvaluationRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<TriggerRiskEvaluationRequest, RiskEvaluationResponse>(RiskEvaluationsBasePath, request, ct: ct);
}
