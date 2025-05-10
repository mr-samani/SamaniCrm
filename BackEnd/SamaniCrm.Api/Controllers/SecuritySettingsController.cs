using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.SecuritySetting.Queries;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
   
    public class SecuritySettingsController : ApiBaseController
    {
        private readonly IMediator  _mediator;

        public SecuritySettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetPasswordComplexity")]
        [ProducesResponseType(typeof(ApiResponse<PasswordComplexityDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPasswordComplexity()
        {
            return ApiOk(await _mediator.Send(new GetPasswordComplexityQuery()));
        }
    }
    
}
