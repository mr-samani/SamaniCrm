using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.MenuCommands;
using SamaniCrm.Application.ProductManager.Commands;
using SamaniCrm.Application.ProductManager.Dtos;
using SamaniCrm.Application.ProductManager.Queries;
using SamaniCrm.Application.ProductManagerManager.Commands;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Queries;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Host.Models;
using System.Linq.Dynamic.Core;

namespace SamaniCrm.Api.Controllers;

[Authorize]
public class ProductController : ApiBaseController
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }


    #region publicApis
    [HttpPost("GetCategories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<ProductCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories([FromBody]GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        List<ProductCategoryDto> result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }

    #endregion




    #region Category
    [HttpPost("GetCategoriesForAdmin")]
    [Permission(AppPermissions.Products.Category.List)]
    [ProducesResponseType(typeof(ApiResponse<PagedProductCategoriesDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoriesForAdmin([FromBody] GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
    {
        PagedProductCategoriesDto result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }


    [HttpPost("GetTreeProductCategoriesForAdmin")]
    [Permission(AppPermissions.Products.Category.List)]
    [ProducesResponseType(typeof(ApiResponse<List<ProductCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTreeProductCategoriesForAdmin([FromBody] GetTreeProductCategoryForAdminQuery request, CancellationToken cancellationToken)
    {
        List<ProductCategoryDto> result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }
    [HttpGet("GetTreeProductCategories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<ProductCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTreeProductCategories([FromBody] GetTreeProductCategoryQuery request, CancellationToken cancellationToken)
    {
        List<ProductCategoryDto> result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }




    [HttpGet("GetProductCategoryForEdit")]
    [Permission(AppPermissions.Products.Category.Edit)]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductCategoryForEdit(Guid id, CancellationToken cancellationToken)
    {
        return ApiOk(await _mediator.Send(new GetProductCategoryForEditQuery(id), cancellationToken));
    }


    [HttpPost("CreateOrEditProductCategory")]
    [Permission(AppPermissions.Products.Category.Edit)]
    [Permission(AppPermissions.Products.Category.Create)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrEditProductCategory([FromBody] CreateOrUpdateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        return ApiOk(await _mediator.Send(request, cancellationToken));
    }

    [HttpPost("DeleteProductCategory")]
    [Permission(AppPermissions.Products.Category.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProductCategory(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return ApiOk<bool>(result);
    }



    [HttpGet("GetAutoCompleteProductCategory")]
    [Permission(AppPermissions.Products.Category.List)]
    [ProducesResponseType(typeof(ApiResponse<List<AutoCompleteDto<Guid>>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAutoCompleteProductCategory(string? filter, CancellationToken cancellationToken)
    {
        List<AutoCompleteDto<Guid>> result = await _mediator.Send(new GetAutoCompleteProductCategoryQuery(filter), cancellationToken);
        return ApiOk(result);
    }

    [HttpGet("GetAllProductCategoryTranslations")]
    [Permission(AppPermissions.Products.Category.Export)]
    [ProducesResponseType(typeof(ApiResponse<List<ExportAllLocalizationValueDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProductCategoryTranslations(CancellationToken cancellationToken)
    {
        List<ExportAllLocalizationValueDto> result = await _mediator.Send(new GetAllProductCategoryTranslationQuery(), cancellationToken);
        return ApiOk(result);
    }
    [HttpPost("ImportProductCategoryLocalization")]
    [Permission(AppPermissions.Products.Category.Import)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ImportProductCategoryLocalization([FromBody] List<ExportAllLocalizationValueDto> data, CancellationToken cancellationToken)
    {
        bool result = await _mediator.Send(new ImportProductCategoryLocalizationCommand(data), cancellationToken);
        return ApiOk(result);
    }

    #endregion
    // ________________________________________________________________________________________________________________________
    #region Types

    [HttpPost("GetProductTypes")]
    [Permission(AppPermissions.Products.Type.List)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductTypes([FromBody] GetProductTypesQuery request, CancellationToken cancellationToken)
    {
        PaginatedResult<ProductTypeDto> result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }

    [HttpGet("GetProductTypeForEdit")]
    [Permission(AppPermissions.Products.Type.Edit)]
    [ProducesResponseType(typeof(ApiResponse<ProductTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductTypeForEdit(Guid id, CancellationToken cancellationToken)
    {
        ProductTypeDto result = await _mediator.Send(new GetProductTypeForEditQuery(id), cancellationToken);
        return ApiOk(result);
    }

    [HttpPost("CreateOrEditProductType")]
    [Permission(AppPermissions.Products.Type.Edit)]
    [Permission(AppPermissions.Products.Type.Create)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrEditProductType([FromBody] CreateOrUpdateProductTypeCommand request, CancellationToken cancellationToken)
    {
        return ApiOk(await _mediator.Send(request, cancellationToken));
    }

    [HttpPost("DeleteProductType")]
    [Permission(AppPermissions.Products.Type.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProductType(DeleteProductTypeCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return ApiOk<bool>(result);
    }


    [HttpGet("GetAutoCompleteProductType")]
    [Permission(AppPermissions.Products.Type.List)]
    [ProducesResponseType(typeof(ApiResponse<List<AutoCompleteDto<Guid>>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAutoCompleteProductType(string? filter, CancellationToken cancellationToken)
    {
        List<AutoCompleteDto<Guid>> result = await _mediator.Send(new GetAutoCompleteProductTypeQuery(filter), cancellationToken);
        return ApiOk(result);
    }
    #endregion
    // ________________________________________________________________________________________________________________________

    #region Attributes

    [HttpPost("GetProductAttributes")]
    [Permission(AppPermissions.Products.Attribute.List)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductAttributeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductAttributes([FromBody] GetProductAttributesQuery request, CancellationToken cancellationToken)
    {
        PaginatedResult<ProductAttributeDto> result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }

    [HttpGet("GetProductAttributeForEdit")]
    [Permission(AppPermissions.Products.Attribute.Edit)]
    [ProducesResponseType(typeof(ApiResponse<ProductAttributeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductAttributeForEdit(Guid id, CancellationToken cancellationToken)
    {
        return ApiOk(await _mediator.Send(new GetProductAttributeForEditQuery(id), cancellationToken));
    }

    [HttpPost("CreateOrEditProductAttribute")]
    [Permission(AppPermissions.Products.Attribute.Edit)]
    [Permission(AppPermissions.Products.Attribute.Create)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrEditProductAttribute([FromBody] CreateOrUpdateProductAttributeCommand request, CancellationToken cancellationToken)
    {
        return ApiOk(await _mediator.Send(request, cancellationToken));
    }

    [HttpPost("DeleteProductAttribute")]
    [Permission(AppPermissions.Products.Attribute.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProductAttribute(DeleteProductAttributeCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return ApiOk<bool>(result);
    }
    #endregion
    // ________________________________________________________________________________________________________________________

    #region product 

    [HttpPost("GetProducts")]
    [Permission(AppPermissions.Products.List)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromBody] GetProductsQuery request, CancellationToken cancellationToken)
    {
        PaginatedResult<ProductListDto> result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }

    [HttpGet("GetProductForEdit")]
    [Permission(AppPermissions.Products.Edit)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductForEdit(Guid id, CancellationToken cancellationToken)
    {
        return ApiOk(await _mediator.Send(new GetProductForEditQuery(id), cancellationToken));
    }

    [HttpPost("CreateOrEditProduct")]
    [Permission(AppPermissions.Products.Edit)]
    [Permission(AppPermissions.Products.Create)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrEditProduct([FromBody] CreateOrUpdateProductCommand request, CancellationToken cancellationToken)
    {
        return ApiOk(await _mediator.Send(request, cancellationToken));
    }

    [HttpPost("DeleteProduct")]
    [Permission(AppPermissions.Products.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProduct(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return ApiOk<bool>(result);
    }
    #endregion
    // ________________________________________________________________________________________________________________________

    #region Currency
    [HttpGet("GetCurrencies")]
    [Permission(AppPermissions.Products.Currency.List)]
    [ProducesResponseType(typeof(ApiResponse<List<CurrencyDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrencies(CancellationToken cancellationToken)
    {
        List<CurrencyDto> result = await _mediator.Send(new GetCurrenciesQuery(), cancellationToken);
        return ApiOk(result);
    }
    [HttpGet("GetActiveCurrencies")]
    [Permission(AppPermissions.Products.Currency.List)]
    [ProducesResponseType(typeof(ApiResponse<List<AutoCompleteDto<string>>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveCurrencies(CancellationToken cancellationToken)
    {
        List<AutoCompleteDto<string>> result = await _mediator.Send(new GetActiveCurrenciesQuery(), cancellationToken);
        return ApiOk(result);
    }

    [HttpPost("CreateOrEditCurrency")]
    [Permission(AppPermissions.Products.Currency.Edit)]
    [Permission(AppPermissions.Products.Currency.Create)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrEditCurrency([FromBody] CreateOrUpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        bool result = await _mediator.Send(request, cancellationToken);
        return ApiOk(result);
    }

    [HttpPost("DeleteCurrency")]
    [Permission(AppPermissions.Products.Currency.Delete)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCurrency(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        bool result = await _mediator.Send(request, cancellationToken);
        return ApiOk<bool>(result);
    }
    #endregion


}
