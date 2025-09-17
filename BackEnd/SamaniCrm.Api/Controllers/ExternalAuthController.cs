using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Application.Auth.Commands;
using SamaniCrm.Application.Auth.Queries;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Api.Controllers;

 
public class ExternalAuthController : ApiBaseController
{
    private readonly IMediator _mediator;

    public ExternalAuthController(IMediator mediator)
    {
        _mediator = mediator;
    }



    [HttpGet("providers")]
    public async Task<IActionResult> GetProviders()
           => Ok(await _mediator.Send(new GetExternalProvidersQuery()));

    [HttpGet("external/challenge")]
    public IActionResult Challenge([FromQuery] string provider, [FromQuery] string returnUrl = "/")
    {
        var props = new AuthenticationProperties { RedirectUri = Url.Action("ExternalCallback") };
        props.Items["returnUrl"] = returnUrl;
        return Challenge(props, provider);
    }

    [HttpGet("external/callback")]
    public async Task<IActionResult> ExternalCallback()
        => Ok(await _mediator.Send(new ExternalLoginCallbackCommand()));
}
