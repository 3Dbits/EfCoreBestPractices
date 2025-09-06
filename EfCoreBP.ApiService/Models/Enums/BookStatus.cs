using System.ComponentModel;
using System.Reflection;

namespace EfCoreBP.ApiService.Models.Enums;

public enum BookStatus
{
    [Description("Book is in store")]
    InStore = 0,

    [Description("Book is borrowed")]
    Borrowed = 1,

    [Description("Nobody knows where the book is")]
    Unknown = 2,

    [Description("Book status is unspecified")]
    Unspecified = 3
}

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}
