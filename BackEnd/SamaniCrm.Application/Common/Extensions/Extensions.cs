using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application
{
    public static class SortingExtensions
    {
        public static bool IsValidSortField<T>(this string? fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return true; // اگر کاربر چیزی نفرستاده باشه، ولید هست

            var validProps = typeof(T).GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(SortableAttribute)))
                .Select(p => p.Name.ToLower())
                .ToHashSet();

            return validProps.Contains(fieldName.ToLower());
        }

        public static IEnumerable<string> GetSortableFields<T>()
        {
            return typeof(T).GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(SortableAttribute)))
                .Select(p => p.Name);
        }
    }

}
