using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/error")]
public class ErrorController : ControllerBase
{
    [HttpGet]
    public IActionResult HandleError()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;

        return Problem(
            title: "An unexpected error occurred",
            detail: exception?.Message,
            statusCode: 500);
    }
}
