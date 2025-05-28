using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Menu.Commands;
using SamaniCrm.Application.Product.Commands;
using SamaniCrm.Application.Product.Dtos;
using SamaniCrm.Application.Product.Queries;
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
        public async Task<IActionResult> CreateOrEditProductCategory(CreateOrUpdateProductCategoryCommand request, CancellationToken cancellationToken)
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

    }
}
