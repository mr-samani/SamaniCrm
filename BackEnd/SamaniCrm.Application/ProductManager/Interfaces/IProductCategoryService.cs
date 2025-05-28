using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Queries;

namespace SamaniCrm.Application.ProductManagerManager.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetCategoryTree(CancellationToken cancellationToken);
        Task<PagedProductCategoriesDto> GetPagedCategories(GetCategoriesForAdminQuery request, CancellationToken cancellationToken);

    }
}
