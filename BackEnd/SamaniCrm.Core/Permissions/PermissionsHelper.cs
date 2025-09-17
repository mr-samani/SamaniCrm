using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Permissions;

namespace SamaniCrm.Core.Shared.Permissions
{
    public static class PermissionsHelper
    {
        public static List<FlatPermission> GetAllPermissions()
        {
            var allPermissions = typeof(AppPermissions).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                             .Where(f => f.FieldType == typeof(string))
                             .Select(f => new FlatPermission()
                             {
                                 LocalizeKey = "Permission_" + f.Name,
                                 Value = f.GetRawConstantValue()?.ToString()!,
                             })
                             .Where(p => !string.IsNullOrEmpty(p.Value))
                             .ToList();

            return allPermissions;
        }
    }


    public class FlatPermission
    {
        public required string LocalizeKey { get; set; }
        public required string Value { get; set; }
    }
}
