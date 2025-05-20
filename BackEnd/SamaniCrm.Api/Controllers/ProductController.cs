using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Product.Dtos;
using SamaniCrm.Application.Product.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetCategoriesForAdmin")]
        [Permission(AppPermissions.Products_Category_List)]
        [ProducesResponseType(typeof(ApiResponse<List<ProductCategoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoriesForAdmin(CancellationToken cancellationToken)
        {
            List<ProductCategoryDto> result = await _mediator.Send(new GetCategoriesForAdminQuery(), cancellationToken);
            return ApiOk<List<ProductCategoryDto>>(result);
        }
    }
}
