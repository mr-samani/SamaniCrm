using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.DTOs
{
    public class PaginatedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } =[];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }



        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public PaginatedResult() { }

        public PaginatedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public PaginatedResult<TResult> Map<TResult>(Func<T, TResult> selector) =>
                          new(Items.Select(selector).ToList(), TotalCount, PageNumber, PageSize);


    }
}
