using CreditSystem.Frontend.Web.Components;
using CreditSystem.Frontend.Web.Services.Api;
using CreditSystem.Frontend.Web.Services.Api.Dummy;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Blazor Web App con render mode Interactive Server (ver nota en el
// .csproj). No se cambia el modelo de hosting sin confirmar con el
// proyecto real existente.
// ---------------------------------------------------------------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ---------------------------------------------------------------------
// Configuracion de acceso a la API (BaseUrl / ApiKey). Ver appsettings.json
// y Services/Api/ApiSettings.cs: BaseUrl y ApiKey aun no confirmadas por
// el equipo de backend; se leen de configuracion para no hardcodearlas.
// ---------------------------------------------------------------------
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));
builder.Services.Configure<DummyDataSettings>(builder.Configuration.GetSection(DummyDataSettings.SectionName));

builder.Services.AddTransient<ApiKeyAuthorizationHandler>();

builder.Services.AddHttpClient(ApiClientConstants.HttpClientName, (sp, client) =>
{
    var apiSettings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiSettings>>().Value;

    if (!string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
    {
        client.BaseAddress = new Uri(apiSettings.BaseUrl);
    }

    client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<ApiKeyAuthorizationHandler>();

// ---------------------------------------------------------------------
// Modo demo (DummyData:Enabled en appsettings): registra DummyApiClient
// en vez de ApiClient. Ambos implementan IApiClient, asi que ningun
// servicio de dominio ni pagina necesita cambiar al alternar el modo —
// solo esta linea decide de donde vienen los datos.
// ---------------------------------------------------------------------
var useDummyData = builder.Configuration.GetValue<bool>($"{DummyDataSettings.SectionName}:Enabled");

if (useDummyData)
{
    builder.Services.AddScoped<IApiClient, DummyApiClient>();
}
else
{
    builder.Services.AddScoped<IApiClient, ApiClient>();
}

// ---------------------------------------------------------------------
// Servicios de dominio (uno por grupo de endpoints documentado). Las
// paginas dependen de estas interfaces, nunca de HttpClient directamente
// (regla explicita del encargo: centralizar la comunicacion HTTP).
// ---------------------------------------------------------------------
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IRevolvingCreditService, RevolvingCreditService>();
builder.Services.AddScoped<IDelinquentLoanService, DelinquentLoanService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGuaranteeService, GuaranteeService>();
builder.Services.AddScoped<IReferenceRateService, ReferenceRateService>();
builder.Services.AddScoped<IRiskService, RiskService>();
builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUnderwritingPolicyService, UnderwritingPolicyService>();
builder.Services.AddScoped<IProjectionAdminService, ProjectionAdminService>();
builder.Services.AddScoped<IRiskEngineService, RiskEngineService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
