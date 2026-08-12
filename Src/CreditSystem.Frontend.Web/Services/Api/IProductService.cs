using CreditSystem.Frontend.Web.Models.Common;
using CreditSystem.Frontend.Web.Models.Requests;
using CreditSystem.Frontend.Web.Models.Responses;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>Grupo "Credit Products" del swagger (base /api/v1/products).</summary>
public interface IProductService
{
    /// <summary>GET /api/v1/products</summary>
    Task<ApiResult<List<ProductResponse>>> GetAllAsync(CancellationToken ct = default);

    /// <summary>POST /api/v1/products</summary>
    Task<ApiResult> CreateAsync(CreateProductRequest request, CancellationToken ct = default);

    /// <summary>GET /api/v1/products/{id}</summary>
    Task<ApiResult<ProductResponse>> GetByIdAsync(Guid productId, CancellationToken ct = default);

    /// <summary>PUT /api/v1/products/{id}/status</summary>
    Task<ApiResult> UpdateStatusAsync(Guid productId, UpdateProductStatusRequest request, CancellationToken ct = default);
}
