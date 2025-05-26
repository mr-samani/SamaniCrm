using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Product.Dtos;
using SamaniCrm.Application.Product.Interfaces;

namespace SamaniCrm.Application.Product.Queries
{
    public class GetCategoriesForAdminQuery : PaginationRequest, IRequest<PaginatedResult<PagedProductCategoryDto>>
    {
        public Guid? ParentId { get; set; }
        public string? Filter { get; set; }
    }


    public class GetCategoriesForAdminQueryHandler : IRequestHandler<GetCategoriesForAdminQuery, PaginatedResult<PagedProductCategoryDto>>
    {
        private readonly IProductCategoryService _productCategoryService;

        public GetCategoriesForAdminQueryHandler(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        public Task<PaginatedResult<PagedProductCategoryDto>> Handle(GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
        {
            return _productCategoryService.GetPagedCategories(request, cancellationToken);
        }
    }
}
