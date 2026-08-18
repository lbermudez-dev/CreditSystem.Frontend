using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Grupo "ProjectionAdmin" (base /api/v1/admin/projection). Endpoint
/// agregado al backend despues del ultimo open_api.json sincronizado (54
/// rutas) — ver CreditSystem.Api/EndPoints/ProjectionAdminEndpoints.cs.
/// Perfil tecnico/administrador: opera sobre el worker de proyecciones
/// (event sourcing) del backend — evaluar si corresponde exponerlo en el
/// frontend de negocio o dejarlo solo para soporte, igual que IAdminService.
/// </summary>
public interface IProjectionAdminService
{
    /// <summary>GET /api/v1/admin/projection/failures?resolved=</summary>
    Task<ApiResult<List<ProjectionFailureResponse>>> GetFailuresAsync(bool resolved = false, CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/projection/failures/{id}/resolve</summary>
    Task<ApiResult<ResolveProjectionFailureResponse>> ResolveFailureAsync(Guid failureId, CancellationToken ct = default);

    /// <summary>
    /// POST /api/v1/admin/projection/rebuild — trunca todos los read models
    /// y reinicia los checkpoints a 0; el worker los reconstruye desde el
    /// event store. Operacion destructiva sobre los read models (no sobre
    /// el event store, que es la fuente de verdad); requiere confirmacion
    /// explicita en la UI antes de invocarla.
    /// </summary>
    Task<ApiResult<RebuildProjectionsResponse>> RebuildAsync(CancellationToken ct = default);
}
