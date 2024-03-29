using System;
using System.Reflection;

namespace Elmanager.Utilities;

internal static class ReflectionUtils
{
    private static PropertyInfo? GetPropertyInfo(Type type, string propertyName)
    {
        PropertyInfo? propInfo;
        var t = type;
        do
        {
            propInfo = type.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            t = t.BaseType;
        } while (propInfo == null && t != null);

        return propInfo;
    }

    internal static object? GetPropertyValue(this object obj, string propertyName)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        var objType = obj.GetType();
        var propInfo = GetPropertyInfo(objType, propertyName);
        if (propInfo == null)
            throw new ArgumentOutOfRangeException(nameof(propertyName),
                $"Couldn't find property {propertyName} in type {objType.FullName}");
        return propInfo.GetValue(obj, null);
    }
}