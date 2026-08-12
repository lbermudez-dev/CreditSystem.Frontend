namespace CreditSystem.Frontend.Web.Models.Common;

/// <summary>
/// Envoltorio para las respuestas binarias del grupo "Documents" del
/// swagger (application/pdf, application/vnd.openxmlformats-...sheet).
/// IApiClient.GetFileAsync lo arma leyendo el Content-Type y el nombre de
/// archivo sugerido (Content-Disposition) de la respuesta HTTP.
/// </summary>
public sealed class DocumentFile
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = "application/octet-stream";
    public string FileName { get; init; } = "documento";
}
