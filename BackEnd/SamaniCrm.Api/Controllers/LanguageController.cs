using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Localize.Commands;
using SamaniCrm.Application.Localize.Queries;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{

    public class LanguageController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public LanguageController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("GetAllLanguages")]
        [ProducesResponseType(typeof(ApiResponse<List<LanguageDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLanguages()
        {
            var result = await _mediator.Send(new GetAllLanguageQuery());
            return ApiOk<List<LanguageDto>>(result);
        }

        [HttpPost("CreateOrUpdate")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdate(CreateOrEditLanguageCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }


        [HttpPost("Delete")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(DeleteLanguageCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }

        [HttpPost("ActiveOrDeactive")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActiveOrDeactive(ActiveOrDeactiveLanguageCommand input)
        {
            var result = await _mediator.Send(input);
            return ApiOk(result);
        }
    }
}
