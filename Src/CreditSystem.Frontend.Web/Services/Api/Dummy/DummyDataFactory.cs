using System.Collections;
using System.Reflection;

namespace CreditSystem.Frontend.Web.Services.Api.Dummy;

/// <summary>
/// Genera instancias "plausibles" de cualquier Response/Request del
/// proyecto por reflexion, sin necesidad de escribir un fixture a mano
/// por cada uno de los 58 endpoints. Rellena cada propiedad publica
/// segun su tipo y, cuando el nombre de la propiedad sugiere su
/// significado (Amount, Status, Email, Currency, etc.), usa un valor
/// coherente con ese significado para que las pantallas se vean creibles.
///
/// Limitacion conocida y aceptada: cada llamada genera datos
/// independientes entre si (ej. el saldo de un prestamo en el listado no
/// coincidira con el de su pantalla de detalle) porque no hay una base de
/// datos simulada detras — es un generador "por forma", pensado para
/// maquetar y ajustar layouts, no para simular reglas de negocio. Si mas
/// adelante se necesita coherencia referencial entre pantallas, esto debe
/// reemplazarse por fixtures explicitos por escenario.
/// </summary>
internal static class DummyDataFactory
{
    private static readonly string[] FirstNames =
    [
        "María", "Carlos", "Ana", "José", "Lucía", "Roberto", "Fernanda", "Miguel",
        "Patricia", "Álvaro", "Gabriela", "Ernesto", "Silvia", "Ramón", "Daniela"
    ];

    private static readonly string[] LastNames =
    [
        "Martínez", "Rodríguez", "González", "Pérez", "Sánchez", "Ramírez", "Torres",
        "Flores", "Morales", "Vargas", "Castro", "Ortega", "Mendoza", "Aguilar"
    ];

    private static readonly string[] StatusPool =
        ["Active", "Approved", "Delinquent", "Restructured", "PaidOff", "Pending"];

    private static readonly string[] CollectionStatusPool =
        ["Contactado", "En Gestión Telefónica", "Notificación Enviada", "Sin Contacto", "Acuerdo de Pago"];

    private static readonly string[] PaymentMethodPool =
        ["Transferencia Bancaria", "Efectivo en Caja", "Débito Automático", "SINPE Móvil"];

    private static readonly string[] RuleNamePool =
        ["CreditScoreRule", "DebtToIncomeRule", "CollateralRule", "MaxLoanAmountRule", "ActiveLoansRule"];

    private static readonly string[] CurrencyPool = ["NIO", "USD"];

    private static readonly string[] GenericWords =
        ["Referencia", "Documento", "Concepto", "Registro", "Detalle", "Proceso", "Información", "Elemento"];

    /// <summary>Crea una instancia de <typeparamref name="T"/> con datos de relleno plausibles.</summary>
    public static T Create<T>(string seed)
    {
        var rng = new Random(StableSeed(seed));
        return (T)CreateValue(typeof(T), string.Empty, rng, 0)!;
    }

    /// <summary>Genera una lista de 3 a 6 elementos de <typeparamref name="T"/>.</summary>
    public static List<T> CreateList<T>(string seed, int? count = null)
    {
        var rng = new Random(StableSeed(seed));
        var n = count ?? rng.Next(3, 7);
        var list = new List<T>(n);
        for (var i = 0; i < n; i++)
        {
            list.Add((T)CreateValue(typeof(T), string.Empty, new Random(StableSeed($"{seed}-{i}")), 0)!);
        }

        return list;
    }

    /// <summary>Hash estable dentro del proceso (no usa string.GetHashCode, que Bootstrap-random-iza por ejecucion).</summary>
    private static int StableSeed(string value)
    {
        unchecked
        {
            var hash = 17;
            foreach (var c in value)
            {
                hash = (hash * 31) + c;
            }

            return hash;
        }
    }

    private static object? CreateValue(Type type, string propertyName, Random rng, int depth)
    {
        if (depth > 4)
        {
            return null;
        }

        var underlying = Nullable.GetUnderlyingType(type);
        var isNullable = underlying is not null;
        var effectiveType = underlying ?? type;

        // Campos de error: en un escenario dummy "feliz" deben quedar vacios.
        if (propertyName.Contains("Error", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (isNullable && rng.NextDouble() < 0.15)
        {
            return null;
        }

        if (effectiveType == typeof(string))
        {
            return CreateString(propertyName, rng);
        }

        if (effectiveType == typeof(Guid))
        {
            var bytes = new byte[16];
            rng.NextBytes(bytes);
            return new Guid(bytes);
        }

        if (effectiveType == typeof(bool))
        {
            var biasToTrue = propertyName.Contains("Success", StringComparison.OrdinalIgnoreCase)
                || propertyName.Contains("Accepted", StringComparison.OrdinalIgnoreCase)
                || propertyName.Contains("Passed", StringComparison.OrdinalIgnoreCase)
                || propertyName.Equals("IsPaid", StringComparison.OrdinalIgnoreCase);

            return biasToTrue ? rng.NextDouble() < 0.9 : rng.Next(2) == 0;
        }

        if (effectiveType == typeof(int))
        {
            return CreateInt(propertyName, rng);
        }

        if (effectiveType == typeof(decimal))
        {
            return CreateDecimal(propertyName, rng);
        }

        if (effectiveType == typeof(double))
        {
            return (double)CreateDecimal(propertyName, rng);
        }

        if (effectiveType == typeof(DateTime))
        {
            return CreateDate(propertyName, rng);
        }

        if (effectiveType == typeof(DateOnly))
        {
            return DateOnly.FromDateTime(CreateDate(propertyName, rng));
        }

        if (effectiveType.IsEnum)
        {
            var values = Enum.GetValues(effectiveType);
            return values.Length == 0 ? null : values.GetValue(rng.Next(values.Length));
        }

        if (effectiveType.IsGenericType && effectiveType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var elementType = effectiveType.GetGenericArguments()[0];
            var list = (IList)Activator.CreateInstance(effectiveType)!;
            var count = rng.Next(4, 8);
            for (var i = 0; i < count; i++)
            {
                list.Add(CreateValue(elementType, propertyName, rng, depth + 1));
            }

            return list;
        }

        if (effectiveType.IsGenericType && effectiveType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var genericArgs = effectiveType.GetGenericArguments();
            var keyType = genericArgs[0];
            var valueType = genericArgs[1];
            var dictionary = (IDictionary)Activator.CreateInstance(effectiveType)!;
            var count = rng.Next(1, 4);
            for (var i = 0; i < count; i++)
            {
                var key = CreateValue(keyType, $"{propertyName}Key", rng, depth + 1);
                if (key is null || dictionary.Contains(key))
                {
                    continue;
                }

                dictionary.Add(key, CreateValue(valueType, $"{propertyName}Value", rng, depth + 1));
            }

            return dictionary;
        }

        if (effectiveType.IsClass && effectiveType != typeof(object))
        {
            return CreateComplex(effectiveType, rng, depth);
        }

        return null;
    }

    private static object CreateComplex(Type type, Random rng, int depth)
    {
        var instance = Activator.CreateInstance(type)!;

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanWrite || prop.GetSetMethod() is null || prop.GetIndexParameters().Length > 0)
            {
                continue;
            }

            var value = CreateValue(prop.PropertyType, prop.Name, rng, depth + 1);
            prop.SetValue(instance, value);
        }

        return instance;
    }

    private static string CreateString(string propertyName, Random rng)
    {
        var name = propertyName;

        bool Is(string token) => name.Contains(token, StringComparison.OrdinalIgnoreCase);

        if (Is("CollectionStatus"))
        {
            return Pick(CollectionStatusPool, rng);
        }

        if (Is("PaymentMethod") || name.Equals("Method", StringComparison.OrdinalIgnoreCase))
        {
            return Pick(PaymentMethodPool, rng);
        }

        if (Is("Status"))
        {
            return Pick(StatusPool, rng);
        }

        if (Is("Currency"))
        {
            return Pick(CurrencyPool, rng);
        }

        if (Is("Email"))
        {
            return $"{FakeFirstName(rng).ToLowerInvariant()}.{FakeLastName(rng).ToLowerInvariant()}@ejemplo-demo.com";
        }

        if (Is("Phone"))
        {
            return $"+505 8{rng.Next(100, 999)}-{rng.Next(1000, 9999)}";
        }

        if (Is("CustomerName") || Is("MemberName") || (Is("Name") && !Is("FileName") && !Is("RuleName")))
        {
            return $"{FakeFirstName(rng)} {FakeLastName(rng)}";
        }

        if (Is("Collector"))
        {
            return $"{FakeFirstName(rng)} {FakeLastName(rng)}";
        }

        if (Is("RuleName"))
        {
            return Pick(RuleNamePool, rng);
        }

        if (Is("MemberNumber"))
        {
            return $"SOC-{rng.Next(1000, 9999)}";
        }

        if (Is("Url"))
        {
            return "https://demo.ejemplo.test/callback";
        }

        if (Is("SecretKey"))
        {
            return Guid.NewGuid().ToString("N")[..16];
        }

        if (Is("Source"))
        {
            return "Banco Central (demo)";
        }

        if (Is("Message"))
        {
            return "Operación completada correctamente (modo demo).";
        }

        if (Is("Reason"))
        {
            return "Ajuste solicitado en modo demo.";
        }

        if (Is("Description"))
        {
            return "Registro generado en modo demo para maquetar la pantalla.";
        }

        if (Is("ContentType"))
        {
            return "application/pdf";
        }

        if (Is("FileName"))
        {
            return "documento-demo.pdf";
        }

        return $"{Pick(GenericWords, rng)} {rng.Next(100, 999)}";
    }

    private static int CreateInt(string propertyName, Random rng)
    {
        bool Is(string token) => propertyName.Contains(token, StringComparison.OrdinalIgnoreCase);

        if (Is("Month") || Is("Term"))
        {
            return rng.Next(6, 61);
        }

        if (Is("Overdue") || Is("Day"))
        {
            return rng.Next(0, 121);
        }

        if (Is("Count") || Is("Number"))
        {
            return rng.Next(1, 51);
        }

        return rng.Next(1, 21);
    }

    private static decimal CreateDecimal(string propertyName, Random rng)
    {
        bool Is(string token) => propertyName.Contains(token, StringComparison.OrdinalIgnoreCase);

        if (Is("Rate") || Is("Percentage") || Is("Ltv") || Is("Spread"))
        {
            return Math.Round((decimal)(rng.NextDouble() * 25), 2);
        }

        return Math.Round((decimal)(rng.NextDouble() * 495_000 + 5_000), 2);
    }

    private static DateTime CreateDate(string propertyName, Random rng)
    {
        bool Is(string token) => propertyName.Contains(token, StringComparison.OrdinalIgnoreCase);

        if (Is("Due") || Is("Expiration") || Is("Effective") || Is("Until") || Is("Vencim"))
        {
            return DateTime.Today.AddDays(rng.Next(1, 90));
        }

        if (Is("Joined") || Is("Accepted") || Is("Created"))
        {
            return DateTime.Today.AddDays(-rng.Next(30, 730));
        }

        return DateTime.Today.AddDays(-rng.Next(0, 60));
    }

    private static string Pick(string[] pool, Random rng) => pool[rng.Next(pool.Length)];

    private static string FakeFirstName(Random rng) => Pick(FirstNames, rng);

    private static string FakeLastName(Random rng) => Pick(LastNames, rng);
}
