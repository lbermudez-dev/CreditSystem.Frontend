using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <inheritdoc cref="IProductService" />
public sealed class ProductService : IProductService
{
    private const string BasePath = "api/v1/products";

    private readonly IApiClient _apiClient;

    public ProductService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<ProductResponse>>> GetAllAsync(CancellationToken ct = default)
        => _apiClient.GetAsync<List<ProductResponse>>(BasePath, ct);

    public Task<ApiResult> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
        => _apiClient.PostAsync(BasePath, request, ct: ct);

    public Task<ApiResult<ProductResponse>> GetByIdAsync(Guid productId, CancellationToken ct = default)
        => _apiClient.GetAsync<ProductResponse>($"{BasePath}/{productId}", ct);

    public Task<ApiResult> UpdateStatusAsync(Guid productId, UpdateProductStatusRequest request, CancellationToken ct = default)
        => _apiClient.PutAsync($"{BasePath}/{productId}/status", request, ct);
}
