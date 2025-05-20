using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Product.Dtos;
using SamaniCrm.Application.Product.Interfaces;

namespace SamaniCrm.Application.Product.Queries
{
    public record GetCategoriesForAdminQuery():IRequest<List<ProductCategoryDto>>;

    public class GetCategoriesForAdminQueryHandler : IRequestHandler<GetCategoriesForAdminQuery, List<ProductCategoryDto>>
    {
        private readonly IProductCategoryService _productCategoryService;

        public GetCategoriesForAdminQueryHandler(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        public Task<List<ProductCategoryDto>> Handle(GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
        {
           return _productCategoryService.GetCategoryTree(cancellationToken);
        }
    }
}
