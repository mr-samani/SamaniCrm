using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace SamaniCrm.Infrastructure.Extensions;
public static class DynamicOrderExtensions
{
    public static IQueryable<T> OrderByDynamic<T>(
        this IQueryable<T> source,
        string sortBy,
        string sortDirection = "asc")
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return source;

        var ordering = $"{sortBy} {sortDirection}";
        return source.OrderBy(ordering);
    }
}

