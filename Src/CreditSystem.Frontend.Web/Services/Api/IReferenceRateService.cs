using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Grupo "Reference Rates" del swagger (base /api/v1/reference-rates).
/// Nota: el "id" es "type: string" (no uuid) en el swagger — a diferencia
/// del resto de identificadores del sistema, que son GUID.
/// </summary>
public interface IReferenceRateService
{
    /// <summary>GET /api/v1/reference-rates/{id}</summary>
    Task<ApiResult<ReferenceRateResponse>> GetAsync(string rateId, CancellationToken ct = default);

    /// <summary>PUT /api/v1/reference-rates/{id}</summary>
    Task<ApiResult<ReferenceRateResponse>> UpdateAsync(string rateId, UpdateReferenceRateRequest request, CancellationToken ct = default);
}
