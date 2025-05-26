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
        // [Permission(AppPermissions.Products_Category_List)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PagedProductCategoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoriesForAdmin([FromBody] GetCategoriesForAdminQuery request, CancellationToken cancellationToken)
        {
            PaginatedResult<PagedProductCategoryDto> result = await _mediator.Send(request, cancellationToken);
            return ApiOk(result);
        }
    }
}
