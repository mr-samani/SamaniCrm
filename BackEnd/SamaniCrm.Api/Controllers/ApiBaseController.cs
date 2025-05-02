using Microsoft.AspNetCore.Mvc;
using SamaniCrm.Host.Models;

namespace SamaniCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiBaseController:ControllerBase
    {
        protected IActionResult ApiOk<T>(T data)
        {
            return Ok(ApiResponse<T>.Ok(data));
        }

        protected IActionResult ApiError(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, ApiResponse<string>.Fail(message));
        }
    }
}
