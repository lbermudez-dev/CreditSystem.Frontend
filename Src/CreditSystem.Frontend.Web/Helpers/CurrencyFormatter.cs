using System.Globalization;

namespace CreditSystem.Frontend.Web.Helpers;

/// <summary>
/// Formatea montos monetarios de forma consistente. El símbolo "C$"
/// corresponde a córdoba nicaragüense (NIO) — se corrigió de una
/// suposición previa que lo asociaba a CRC (colón costarricense).
/// </summary>
public static class CurrencyFormatter
{
    /// <summary>
    /// DOCUMENTACION INSUFICIENTE: varios responses del swagger real
    /// (ej. LoanSummaryResponse, DelinquentLoanResponse, DefaultedLoanResponse,
    /// PaidOffLoanResponse) NO incluyen un campo "currency" — solo montos
    /// decimales sueltos. Se usa esta moneda por defecto para mostrarlos
    /// mientras se confirma si el sistema opera en una sola moneda o si
    /// falta el campo en la respuesta real.
    /// </summary>
    public const string DefaultCurrency = "NIO";

    public static string Format(decimal amount, string? currency = null)
    {
        var code = string.IsNullOrWhiteSpace(currency) ? DefaultCurrency : currency.ToUpperInvariant();

        var symbol = code switch
        {
            "NIO" => "C$",
            "USD" => "$",
            "CRC" => "₡",
            _ => code + " "
        };

        return $"{symbol} {amount.ToString("N2", CultureInfo.InvariantCulture)}";
    }
}
