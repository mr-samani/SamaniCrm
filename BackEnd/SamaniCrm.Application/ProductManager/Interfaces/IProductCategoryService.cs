using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.ProductManager.Queries;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetCategoryTree(CancellationToken cancellationToken);
        Task<PagedProductCategoriesDto> GetPagedCategories(GetCategoriesForAdminQuery request, CancellationToken cancellationToken);

        Task<List<ProductCategoryDto>> GetPublicCategories(GetProductCategoriesQuery request, CancellationToken cancellationToken);

    }
}
