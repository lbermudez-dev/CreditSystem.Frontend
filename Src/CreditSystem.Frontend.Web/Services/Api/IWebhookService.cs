using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Webhooks" del swagger (base /api/v1/webhooks).</summary>
public interface IWebhookService
{
    /// <summary>POST /api/v1/webhooks/subscribe</summary>
    Task<ApiResult<SubscribeWebhookResponse>> SubscribeAsync(SubscribeWebhookRequest request, CancellationToken ct = default);
}
