using System.Web;

namespace CreditSystem.Frontend.Web.Services.Api;

/// <summary>
/// Arma query strings a partir de valores opcionales, evitando repetir el
/// mismo bloque de "si tiene valor, agregarlo" en cada servicio. Los
/// nombres de parametros deben coincidir EXACTO con los del swagger
/// (sensible a mayusculas/minusculas del lado del backend).
/// </summary>
public sealed class QueryStringBuilder
{
    private readonly System.Collections.Specialized.NameValueCollection _query = HttpUtility.ParseQueryString(string.Empty);

    public QueryStringBuilder Add(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _query[name] = value;
        }
        return this;
    }

    public QueryStringBuilder Add(string name, int? value)
    {
        if (value.HasValue)
        {
            _query[name] = value.Value.ToString();
        }
        return this;
    }

    public QueryStringBuilder Add(string name, bool? value)
    {
        if (value.HasValue)
        {
            _query[name] = value.Value ? "true" : "false";
        }
        return this;
    }

    public QueryStringBuilder Add(string name, DateTime? value)
    {
        if (value.HasValue)
        {
            _query[name] = value.Value.ToString("o");
        }
        return this;
    }

    public string BuildUrl(string basePath)
    {
        var qs = _query.ToString();
        return qs is { Length: > 0 } ? $"{basePath}?{qs}" : basePath;
    }
}
