using System.Reflection;

namespace MercuryLibrary.Extensions;

public static partial class Extensions
{
    public static T? GetAttribute<T>(this Enum value) where T : Attribute
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<T>();
        return attribute;
    }
}
