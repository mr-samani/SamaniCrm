using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Api.Attributes;
using SamaniCrm.Application.DataSourceManager.Dtos;
using SamaniCrm.Application.DataSourceManager.Queries;
using SamaniCrm.Application.FileManager.Dtos;
using SamaniCrm.Application.FileManager.Queries;
using SamaniCrm.Core.Permissions;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    public class DataSourceController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public DataSourceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetTreeDataSourceDefinitions")]
        [Permission(AppPermissions.PageBuilder_Edit)]
        [ProducesResponseType(typeof(ApiResponse<List<DataSourceDefinitionTreeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeDataSourceDefinitions()
        {
            List<DataSourceDefinitionTreeDto> result = await _mediator.Send(new GetDefinitionsTreeQuery());
            return ApiOk(result);
        }
    }
}
