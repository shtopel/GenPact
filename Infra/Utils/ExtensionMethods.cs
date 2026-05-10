using System.ComponentModel;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetValue(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}