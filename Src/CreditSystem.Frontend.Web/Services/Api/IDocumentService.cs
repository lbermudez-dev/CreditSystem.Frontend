using CreditSystem.Frontend.Web.Models.Common;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Grupo "Documents" del swagger (base /api/v1/loans/{id}/documents).
/// A diferencia del resto, estas respuestas NO son JSON (application/pdf o
/// application/vnd.openxmlformats-officedocument.spreadsheetml.sheet).
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// GET /api/v1/loans/{id}/documents/amortization-table?format=
    /// DOCUMENTACION INSUFICIENTE: el swagger no documenta los valores
    /// validos de "format" (probablemente "pdf" / "xlsx"); confirmar.
    /// </summary>
    Task<ApiResult<DocumentFile>> GetAmortizationTableAsync(Guid loanId, string format, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/{id}/documents/balance-letter</summary>
    Task<ApiResult<DocumentFile>> GetBalanceLetterAsync(Guid loanId, CancellationToken ct = default);

    /// <summary>GET /api/v1/loans/{id}/documents/payment-receipt/{paymentId}</summary>
    Task<ApiResult<DocumentFile>> GetPaymentReceiptAsync(Guid loanId, Guid paymentId, CancellationToken ct = default);
}
