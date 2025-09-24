using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Interfaces;

namespace SamaniCrm.Application.ProductManagerManager.Queries
{
    public class GetCategoriesForAdminQuery : PaginationRequest, IRequest<PagedProductCategoriesDto>
    {
        public Guid? ParentId { get; set; }
        public string? Filter { get; set; }
    }

    public class PagedProductCategoriesDto : PaginatedResult<ProductCategoryDto>
    {
        public List<BreadcrumbResult> Breadcrumbs { get; set; } = default!;
    }

    public class BreadcrumbResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
    }


    public class GetCategoriesForAdminQueryHandler : IRequestHandler<GetCategoriesForAdminQuery, PagedProductCategoriesDto>
    {
        private readonly IProductCategoryService _productCategoryService;

        public GetCategoriesForAdminQueryHandler(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        public Task<PagedProductCategoriesDto> Handle(GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
        {
            return _productCategoryService.GetPagedCategories(request, cancellationToken);
        }
    }
}
