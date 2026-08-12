using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IWebhookService" />
public sealed class WebhookService : IWebhookService
{
    private const string BasePath = "api/v1/webhooks";

    private readonly IApiClient _apiClient;

    public WebhookService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<SubscribeWebhookResponse>> SubscribeAsync(SubscribeWebhookRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync<SubscribeWebhookRequest, SubscribeWebhookResponse>($"{BasePath}/subscribe", request, ct: ct);
}
