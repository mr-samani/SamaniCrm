using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Menu.Commands;
using SamaniCrm.Application.ProductManager.Commands;
using SamaniCrm.Application.ProductManager.Queries;
using SamaniCrm.Application.ProductManagerManager.Commands;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Application.ProductManagerManager.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;
using System.Linq.Dynamic.Core;

namespace SamaniCrm.Api.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #region Category
        [HttpPost("GetCategoriesForAdmin")]
        [Permission(AppPermissions.Products_Category_List)]
        [ProducesResponseType(typeof(ApiResponse<PagedProductCategoriesDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoriesForAdmin([FromBody] GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
        {
            PagedProductCategoriesDto result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetProductCategoryForEdit")]
        [Permission(AppPermissions.Products_Category_Edit)]
        [ProducesResponseType(typeof(ApiResponse<ProductCategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductCategoryForEdit(Guid id, CancellationToken cancellationToken)
        {
            return ApiOk(await _mediator.Send(new GetProductCategoryForEditQuery(id), cancellationToken));
        }


        [HttpPost("CreateOrEditProductCategory")]
        [Permission(AppPermissions.Products_Category_Edit)]
        [Permission(AppPermissions.Products_Category_Create)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditProductCategory([FromBody] CreateOrUpdateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            return ApiOk(await _mediator.Send(request, cancellationToken));
        }

        [HttpPost("DeleteProductCategory")]
        [Permission(AppPermissions.Products_Category_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProductCategory(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return ApiOk<bool>(result);
        }



        [HttpGet("GetAutoCompleteProductCategory")]
        [Permission(AppPermissions.Products_Category_List)]
        [ProducesResponseType(typeof(ApiResponse<List<AutoCompleteDto<Guid>>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAutoCompleteProductCategory(string? filter, CancellationToken cancellationToken)
        {
            List<AutoCompleteDto<Guid>> result = await _mediator.Send(new GetAutoCompleteProductCategoryQuery(filter), cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetAllProductCategoryTranslations")]
        [Permission(AppPermissions.Products_Category_List)]
        [ProducesResponseType(typeof(ApiResponse<List<ExportAllLocalizationValueDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProductCategoryTranslations(CancellationToken cancellationToken)
        {
            List<ExportAllLocalizationValueDto> result = await _mediator.Send(new GetAllProductCategoryTranslationQuery(), cancellationToken);
            return ApiOk(result);
        }
        [HttpPost("ImportProductCategoryLocalization")]
        [Permission(AppPermissions.Products_Category_List)]
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
        [Permission(AppPermissions.Products_Type_List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductTypeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductTypes([FromBody] GetProductTypesQuery request, CancellationToken cancellationToken)
        {
            PaginatedResult<ProductTypeDto> result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetProductTypeForEdit")]
        [Permission(AppPermissions.Products_Type_Edit)]
        [ProducesResponseType(typeof(ApiResponse<ProductTypeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductTypeForEdit(Guid id, CancellationToken cancellationToken)
        {
            ProductTypeDto result = await _mediator.Send(new GetProductTypeForEditQuery(id), cancellationToken);
            return ApiOk(result);
        }

        [HttpPost("CreateOrEditProductType")]
        [Permission(AppPermissions.Products_Type_Edit)]
        [Permission(AppPermissions.Products_Type_Create)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditProductType([FromBody] CreateOrUpdateProductTypeCommand request, CancellationToken cancellationToken)
        {
            return ApiOk(await _mediator.Send(request, cancellationToken));
        }

        [HttpPost("DeleteProductType")]
        [Permission(AppPermissions.Products_Type_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProductType(DeleteProductTypeCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return ApiOk<bool>(result);
        }


        [HttpGet("GetAutoCompleteProductType")]
        [Permission(AppPermissions.Products_Type_List)]
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
        [Permission(AppPermissions.Products_Attribute_List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductAttributeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductAttributes([FromBody] GetProductAttributesQuery request, CancellationToken cancellationToken)
        {
            PaginatedResult<ProductAttributeDto> result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetProductAttributeForEdit")]
        [Permission(AppPermissions.Products_Attribute_Edit)]
        [ProducesResponseType(typeof(ApiResponse<ProductAttributeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductAttributeForEdit(Guid id, CancellationToken cancellationToken)
        {
            return ApiOk(await _mediator.Send(new GetProductAttributeForEditQuery(id), cancellationToken));
        }

        [HttpPost("CreateOrEditProductAttribute")]
        [Permission(AppPermissions.Products_Attribute_Edit)]
        [Permission(AppPermissions.Products_Attribute_Create)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditProductAttribute([FromBody] CreateOrUpdateProductAttributeCommand request, CancellationToken cancellationToken)
        {
            return ApiOk(await _mediator.Send(request, cancellationToken));
        }

        [HttpPost("DeleteProductAttribute")]
        [Permission(AppPermissions.Products_Attribute_Delete)]
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
        [Permission(AppPermissions.Products_List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromBody] GetProductsQuery request, CancellationToken cancellationToken)
        {
            PaginatedResult<ProductDto> result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }

        [HttpGet("GetProductForEdit")]
        [Permission(AppPermissions.Products_Edit)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductForEdit(Guid id, CancellationToken cancellationToken)
        {
            return ApiOk(await _mediator.Send(new GetProductForEditQuery(id), cancellationToken));
        }

        [HttpPost("CreateOrEditProduct")]
        [Permission(AppPermissions.Products_Edit)]
        [Permission(AppPermissions.Products_Create)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrEditProduct([FromBody] CreateOrUpdateProductCommand request, CancellationToken cancellationToken)
        {
            return ApiOk(await _mediator.Send(request, cancellationToken));
        }

        [HttpPost("DeleteProduct")]
        [Permission(AppPermissions.Products_Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProduct(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return ApiOk<bool>(result);
        }
        #endregion
        // ________________________________________________________________________________________________________________________

    }
}
