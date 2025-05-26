using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Product.Dtos;
using SamaniCrm.Application.Product.Queries;

namespace SamaniCrm.Application.Product.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetCategoryTree(CancellationToken cancellationToken);
        Task<PaginatedResult<PagedProductCategoryDto>> GetPagedCategories(GetCategoriesForAdminQuery request, CancellationToken cancellationToken);

    }
}
