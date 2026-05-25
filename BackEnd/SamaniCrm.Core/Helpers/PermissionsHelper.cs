using SamaniCrm.Core.Shared.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.Helpers;

public static class PermissionsHelper
{

    /// <summary>
    ///  همه فیلدهای عمومی و static از کلاس AppPermissions را برگردان
    /// </summary>
    /// <returns></returns>
    ////public static List<FlatPermission> GetAllPermissions()
    ////{
    ////    var allPermissions = typeof(AppPermissions).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
    ////                     .Where(f => f.FieldType == typeof(string))
    ////                     .Select(f => new FlatPermission()
    ////                     {
    ////                         LocalizeKey = "Permission_" + f.Name,
    ////                         Value = f.GetRawConstantValue()?.ToString()!,
    ////                     })
    ////                     .Where(p => !string.IsNullOrEmpty(p.Value))
    ////                     .ToList();

    ////    return allPermissions;
    ////}




    public static List<FlatPermission> GetAllPermissions()
    {
        var allPermissions = new List<FlatPermission>();
        CollectPermissionsRecursive(typeof(AppPermissions), null, allPermissions);
        return allPermissions;
    }

    private static void CollectPermissionsRecursive(
        Type type,
        string? parentCategory,
        List<FlatPermission> result)
    {
        // فیلدهای constant string در این کلاس
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.FieldType == typeof(string));

        foreach (var field in fields)
        {
            var value = field.GetRawConstantValue()?.ToString();
            if (string.IsNullOrEmpty(value)) continue;

            var category = parentCategory ?? type.Name;

            // اگر فیلد از کلاس دیگری آمده، نام آن کلاس را بگیر
            if (field.DeclaringType != type)
            {
                category = field.DeclaringType?.Name ?? category;
            }

            result.Add(new FlatPermission
            {
                //LocalizeKey = $"Permission_{field.Name}",
                LocalizeKey = $"Permission_{value.Replace(".", "_")}",
                Value = value,
                Category = category
            });
        }

        // Nested types را هم بررسی کن
        var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
        foreach (var nestedType in nestedTypes)
        {
            CollectPermissionsRecursive(nestedType, nestedType.Name, result);
        }
    }

    public static Dictionary<string, List<FlatPermission>> GetPermissionsByCategory()
    {
        return GetAllPermissions()
            .GroupBy(p => p.Category)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}

public class FlatPermission
{
    public required string LocalizeKey { get; set; }
    public required string Value { get; set; }
    public required string Category { get; set; }
}