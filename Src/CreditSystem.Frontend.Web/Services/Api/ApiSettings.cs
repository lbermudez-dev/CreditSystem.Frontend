namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Configuracion de acceso a la API del backend (seccion "Api" en appsettings).
/// BaseUrl, ApiKey y el nombre del header son provisionales: el usuario confirmo
/// que la autenticacion "posiblemente" sera por ApiKey pero aun no hay
/// confirmacion definitiva del header exacto ni de la BaseUrl real.
/// </summary>
public sealed class ApiSettings
{
    public const string SectionName = "Api";

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del header HTTP usado para enviar la ApiKey.
    /// DOCUMENTACION INSUFICIENTE: se asume "X-Api-Key" por convencion;
    /// confirmar con el equipo de backend el nombre real (podria ser
    /// "Authorization: ApiKey ..." u otro esquema).
    /// </summary>
    public string ApiKeyHeaderName { get; set; } = "X-Api-Key";

    public int TimeoutSeconds { get; set; } = 30;
}
