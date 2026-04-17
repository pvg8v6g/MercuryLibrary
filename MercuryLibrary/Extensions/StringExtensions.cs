using System.Diagnostics.CodeAnalysis;

namespace MercuryLibrary.Extensions;

public static partial class Extensions
{
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return value is null || value.Length is 0;
    }
}
