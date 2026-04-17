// ReSharper disable InconsistentNaming

namespace MercuryLibrary.Extensions;

public static partial class Extensions
{
    public static V? GetOrDefault<K, V>(this Dictionary<K, V> value, K key, V? defaultValue = default) where K : notnull
    {
        return value.TryGetValue(key, out var v) ? v : defaultValue;
    }
}
