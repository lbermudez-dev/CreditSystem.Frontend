namespace CreditSystem.Frontend.Web.Models.Common;

/// <summary>
/// Envoltorio uniforme para el resultado de cualquier llamada a la API.
/// Permite a las paginas distinguir Loading / Success / Empty / Error sin
/// propagar excepciones crudas hacia la UI.
/// </summary>
public sealed class ApiResult<T>
{
    public bool IsSuccess { get; private init; }
    public T? Data { get; private init; }
    public ApiError? Error { get; private init; }

    public static ApiResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static ApiResult<T> Failure(ApiError error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}

/// <summary>
/// Version sin payload, para operaciones tipo POST/DELETE que solo
/// necesitan confirmar exito/fracaso (ej. congelar linea, cerrar linea).
/// </summary>
public sealed class ApiResult
{
    public bool IsSuccess { get; private init; }
    public ApiError? Error { get; private init; }

    public static ApiResult Success() => new() { IsSuccess = true };

    public static ApiResult Failure(ApiError error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}

public sealed class ApiError
{
    public int? StatusCode { get; init; }
    public string Message { get; init; } = "Ocurrio un error inesperado.";

    /// <summary>
    /// Errores de validacion por campo, cuando el backend los devuelve
    /// estructurados (formato exacto no documentado en los .md; se
    /// contempla el caso mas comun: diccionario campo -> lista de mensajes).
    /// </summary>
    public IDictionary<string, string[]>? ValidationErrors { get; init; }

    public bool IsUnauthorized => StatusCode == 401;
    public bool IsForbidden => StatusCode == 403;
    public bool IsNotFound => StatusCode == 404;
    public bool IsConflict => StatusCode == 409;
    public bool IsValidation => StatusCode == 400 || StatusCode == 422;
}
