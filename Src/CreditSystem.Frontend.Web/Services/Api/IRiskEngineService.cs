using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Grupo "RiskEngine" del swagger CRM (openapi_crm.txt, Crm.WebApi), base
/// /api/v1/risk-rules, /api/v1/risk-matrices y /api/v1/risk-evaluations.
/// No confundir con IRiskService (grupo "Risk Classification" de
/// CreditSystem.Api, /api/v1/loans/risk-summary — dominio distinto).
/// DOCUMENTACION INSUFICIENTE: se asume que Crm.WebApi esta detras del
/// mismo gateway/BaseUrl/ApiKey que CreditSystem.Api, por eso este
/// servicio delega en el mismo IApiClient sin config propia.
/// </summary>
public interface IRiskEngineService
{
    /// <summary>GET /api/v1/risk-rules</summary>
    Task<ApiResult<List<RiskRuleResponse>>> ListRiskRulesAsync(CancellationToken ct = default);

    /// <summary>GET /api/v1/risk-rules/{id}</summary>
    Task<ApiResult<RiskRuleResponse>> GetRiskRuleAsync(Guid id, CancellationToken ct = default);

    /// <summary>POST /api/v1/risk-rules</summary>
    Task<ApiResult<RiskRuleResponse>> CreateRiskRuleAsync(CreateRiskRuleRequest request, CancellationToken ct = default);

    /// <summary>PUT /api/v1/risk-rules/{id}</summary>
    Task<ApiResult<RiskRuleResponse>> UpdateRiskRuleAsync(Guid id, UpdateRiskRuleRequest request, CancellationToken ct = default);

    /// <summary>DELETE /api/v1/risk-rules/{id}</summary>
    Task<ApiResult> DeleteRiskRuleAsync(Guid id, CancellationToken ct = default);

    /// <summary>GET /api/v1/risk-matrices</summary>
    Task<ApiResult<List<RiskMatrixResponse>>> ListRiskMatricesAsync(CancellationToken ct = default);

    /// <summary>GET /api/v1/risk-matrices/{id}</summary>
    Task<ApiResult<RiskMatrixDetailResponse>> GetRiskMatrixAsync(Guid id, CancellationToken ct = default);

    /// <summary>POST /api/v1/risk-matrices</summary>
    Task<ApiResult<RiskMatrixResponse>> CreateRiskMatrixAsync(CreateRiskMatrixRequest request, CancellationToken ct = default);

    /// <summary>PUT /api/v1/risk-matrices/{id}</summary>
    Task<ApiResult<RiskMatrixResponse>> UpdateRiskMatrixAsync(Guid id, UpdateRiskMatrixRequest request, CancellationToken ct = default);

    /// <summary>DELETE /api/v1/risk-matrices/{id}</summary>
    Task<ApiResult> DeleteRiskMatrixAsync(Guid id, CancellationToken ct = default);

    /// <summary>POST /api/v1/risk-matrices/{id}/activate</summary>
    Task<ApiResult> ActivateRiskMatrixAsync(Guid id, CancellationToken ct = default);

    /// <summary>POST /api/v1/risk-evaluations</summary>
    Task<ApiResult<RiskEvaluationResponse>> TriggerRiskEvaluationAsync(TriggerRiskEvaluationRequest request, CancellationToken ct = default);
}
