using System;
using Microsoft.AspNetCore.Mvc;

public static class UrlHelperExtensions
{
    public static string Action<T>(this IUrlHelper helper, Func<T, string> getActionName) where T : ControllerBase
    {
        var controller = GetName(typeof(T).Name);
        var actionName = getActionName(null!);
        var url = helper.Action(actionName, controller);
        return url!;
    }

    public static string Action<T>(this IUrlHelper helper, Func<T, string> getActionName, object values) where T : ControllerBase
    {
        var controller = GetName(typeof(T).Name);
        var actionName = getActionName(null!);
        var url = helper.Action(actionName, controller, values);
        return url!;
    }

    public static string Action<T>(this IUrlHelper helper, Func<T, string> getActionName, object values, string protocol) where T : ControllerBase
    {
        var controller = GetName(typeof(T).Name);
        var actionName = getActionName(null!);
        var url = helper.Action(actionName, controller, values, protocol);
        return url!;
    }

    // Вырезает из конца названия "Controller"
    private static string GetName(string name)
    {
        const string Word = "Controller";
        if (name.EndsWith(Word, StringComparison.OrdinalIgnoreCase))
        {
            return name.Remove(name.Length - Word.Length);
        }
        return name;
    }

}
