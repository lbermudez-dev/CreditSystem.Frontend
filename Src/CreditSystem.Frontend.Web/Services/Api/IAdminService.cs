using CreditSystem.Frontend.Web.Models.Common;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Grupo "Admin" del swagger: jobs manuales de operacion interna (base
/// /api/v1/admin). Perfil tecnico/administrador — evaluar si corresponde
/// exponerlos en el frontend de negocio o dejarlos solo para soporte.
/// </summary>
public interface IAdminService
{
    /// <summary>POST /api/v1/admin/jobs/interest-accrual</summary>
    Task<ApiResult> RunInterestAccrualJobAsync(CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/loans/{id}/accrue-interest</summary>
    Task<ApiResult> AccrueInterestForLoanAsync(Guid loanId, CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/jobs/payment-missed</summary>
    Task<ApiResult> RunPaymentMissedJobAsync(CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/jobs/revolving-interest-accrual</summary>
    Task<ApiResult> RunRevolvingInterestAccrualJobAsync(CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/jobs/statement-generation</summary>
    Task<ApiResult> RunStatementGenerationJobAsync(CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/jobs/revolving-payment-missed</summary>
    Task<ApiResult> RunRevolvingPaymentMissedJobAsync(CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/revolving-credits/{id}/accrue-interest</summary>
    Task<ApiResult> AccrueInterestForCreditLineAsync(Guid creditLineId, CancellationToken ct = default);

    /// <summary>POST /api/v1/admin/revolving-credits/{id}/generate-statement</summary>
    Task<ApiResult> GenerateStatementForCreditLineAsync(Guid creditLineId, CancellationToken ct = default);
}
