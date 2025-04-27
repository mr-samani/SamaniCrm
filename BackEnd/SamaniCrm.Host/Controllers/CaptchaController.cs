using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Captcha.Queries;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class CaptchaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CaptchaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("reload")]
        public async Task<ActionResult<CaptchaDto>> Reload()
        {
            var captcha = await _mediator.Send(new GetCaptchaQuery());
            return Ok(captcha);
        }
    }
}
