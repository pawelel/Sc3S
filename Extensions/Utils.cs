using Microsoft.AspNetCore.Components;

using MudBlazor;

using Sc3S.Enumerations;

using System.Collections.Specialized;

using System.Reflection;

using System.Web;

namespace Sc3S.Extensions;

public static class Utils
{
    public static readonly int[] PageSizeOptions = { 5, 10, 25, 50, 100 };

    public static Color SetColor(Status status)
    {
        var color = Color.Default;
        switch (status)
        {
            case Status.Retired:
                color = Color.Error;
                break;

            case Status.InUse:
                color = Color.Default;
                break;

            case Status.InStock:
                color = Color.Success;
                break;

            case Status.Unknown:
                color = Color.Warning;
                break;

            case Status.InRepair:
                color = Color.Secondary;
                break;
        }
        return color;
    }

    public static string WarnMissingValue(string? style)
    {
        return string.IsNullOrEmpty(style) || style.Contains("Unknown") ? "background-color:#ff9800ff; color" : string.Empty;
    }

    public static string MissingTextCheck(string? value)
    {
        return string.IsNullOrEmpty(value) ? "Brak danych" : value;
    }

    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    public static string QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager.QueryString()[key] ?? string.Empty;
    }

    public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
    {
        var result = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<TAttribute>();
        return result!;
    }
}