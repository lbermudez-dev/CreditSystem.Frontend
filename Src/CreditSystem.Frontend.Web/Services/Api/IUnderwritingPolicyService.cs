using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Grupo "Underwriting Policies" (base /api/v1/underwriting-policies).
/// Endpoint agregado al backend despues del ultimo open_api.json
/// sincronizado (54 rutas) — ver CreditSystem.Api/EndPoints/UnderwritingPolicyEndpoints.cs.
/// Expone el catalogo de politicas de originacion (tasa base, DTI maximo,
/// dias de gracia, etc.) que hoy se referencian por Id desde CreditProduct
/// pero no tienen pantalla propia todavia.
/// </summary>
public interface IUnderwritingPolicyService
{
    /// <summary>GET /api/v1/underwriting-policies</summary>
    Task<ApiResult<List<UnderwritingPolicyResponse>>> GetAllAsync(CancellationToken ct = default);
}
