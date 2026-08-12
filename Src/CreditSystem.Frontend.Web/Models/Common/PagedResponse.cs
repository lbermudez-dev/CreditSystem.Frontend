namespace CreditSystem.Frontend.Web.Models.Common;

/// <summary>
/// DOCUMENTACION INSUFICIENTE: los .md no confirman si los listados
/// (GET /api/loans/defaulted, /api/delinquent-loans, /api/loans/paid-off,
/// etc.) devuelven un arreglo plano (IList[...]Response, tal como se
/// documenta literalmente en backend-overview.md) o una respuesta paginada.
/// Se define este envoltorio generico por si el backend real pagina, pero
/// los servicios (ver Services/Api) deserializan por defecto como arreglo
/// plano IList[T], que es lo que dice la documentacion. Ajustar el
/// deserializador en ApiClient si el backend real usa paginacion.
/// </summary>
public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
