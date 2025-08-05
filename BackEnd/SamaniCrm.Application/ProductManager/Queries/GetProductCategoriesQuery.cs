using MediatR;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManager.Queries;


/// <summary>
///  دریافت لیست دسته بندی ها برای سایت اصلی
///  
/// بدون احراز هویت
/// </summary>
public class GetProductCategoriesQuery : IRequest<List<ProductCategoryDto>>
{
    public Guid? ParentId { get; set; }
    public string? Filter { get; set; }
    [DefaultValue(0)]
    public int Skip { get; set; } = 0;
    [DefaultValue(10)]
    public int Take { get; set; } = 10;
    
}


public class GetCategoriesQueryHandler : IRequestHandler<GetProductCategoriesQuery, List<ProductCategoryDto>>
{
    private readonly IProductCategoryService _productCategoryService;

    public GetCategoriesQueryHandler(IProductCategoryService productCategoryService)
    {
        _productCategoryService = productCategoryService;
    }

    public Task<List<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        return _productCategoryService.GetPublicCategories(request, cancellationToken);
    }
}
