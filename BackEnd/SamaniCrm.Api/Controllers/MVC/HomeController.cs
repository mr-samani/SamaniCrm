using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace SamaniCrm.Api.Controllers.MVC;

[Route("[controller]")]
public class HomeController : Controller
{


    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;

    public HomeController(
        IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
    {
        _interaction = interaction;
        _environment = environment;
    }



    public string Index()
    {
        return "OK";
    }



    [HttpGet("error")]
    public async Task<IActionResult> Error(string errorId, CancellationToken cancellation)
    {
        var error =
            await _interaction.GetErrorContextAsync(errorId);

        if (error == null)
        {
            return NotFound();
        }

        // ساخت ViewModel برای View
        var model = new ErrorViewModel
        {
            Error = new ErrorMessage
            {
                Error = error.Error,
                ErrorDescription = error.ErrorDescription,
                ClientId = error.ClientId,
                RedirectUri = error.RedirectUri
            }
        };

        // برگرداندن View به جای Json
        return View("Error", model);

    }

}



public class ErrorViewModel
{
    public ErrorViewModel()
    {
    }

    public ErrorViewModel(string error)
    {
        Error = new Duende.IdentityServer.Models.ErrorMessage { Error = error };
    }

    public Duende.IdentityServer.Models.ErrorMessage? Error { get; set; }
}

// کلاس کمکی برای نمایش اطلاعات خطا
public class ErrorInfo
{
    public string? Error { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ClientId { get; set; }
    public string? RedirectUri { get; set; }
}
