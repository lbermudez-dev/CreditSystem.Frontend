namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Interruptor global de modo demo (seccion "DummyData" en appsettings).
/// Con <see cref="Enabled"/> = true, <see cref="Dummy.DummyApiClient"/> se
/// registra en vez de <see cref="ApiClient"/> (ver Program.cs) y NINGUNA
/// pagina ni servicio de dominio necesita cambiar: ambos implementan
/// <see cref="IApiClient"/> y las paginas solo conocen esa interfaz.
/// Para pasar a producción con el backend real basta con poner
/// "DummyData:Enabled" en false (o quitar la seccion).
/// </summary>
public sealed class DummyDataSettings
{
    public const string SectionName = "DummyData";

    public bool Enabled { get; set; }
}
