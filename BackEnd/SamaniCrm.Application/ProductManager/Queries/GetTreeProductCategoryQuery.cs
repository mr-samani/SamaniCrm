using MediatR;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries;

public record GetTreeProductCategoryQuery() : IRequest<List<ProductCategoryDto>>;




public class GetTreeProductCategoryQueryHandler : IRequestHandler<GetTreeProductCategoryQuery, List<ProductCategoryDto>>
{
    private readonly IProductCategoryService _productCategoryService;

    public GetTreeProductCategoryQueryHandler(IProductCategoryService productCategoryService)
    {
        _productCategoryService = productCategoryService;
    }

    public Task<List<ProductCategoryDto>> Handle(GetTreeProductCategoryQuery request, CancellationToken cancellationToken)
    {
        return _productCategoryService.GetTreeProductCategories(true, cancellationToken);
    }
}




/*---------------------------------------------------------------------------------------*/


public record GetTreeProductCategoryForAdminQuery() : IRequest<List<ProductCategoryDto>>;




public class GetTreeProductCategoryForAdminQueryHandler : IRequestHandler<GetTreeProductCategoryForAdminQuery, List<ProductCategoryDto>>
{
    private readonly IProductCategoryService _productCategoryService;

    public GetTreeProductCategoryForAdminQueryHandler(IProductCategoryService productCategoryService)
    {
        _productCategoryService = productCategoryService;
    }

    public Task<List<ProductCategoryDto>> Handle(GetTreeProductCategoryForAdminQuery request, CancellationToken cancellationToken)
    {
        return _productCategoryService.GetTreeProductCategories(false, cancellationToken);
    }
}




