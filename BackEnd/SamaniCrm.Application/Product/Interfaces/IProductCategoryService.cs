using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.Product.Dtos;

namespace SamaniCrm.Application.Product.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetCategoryTree(CancellationToken cancellationToken);
    }
}
