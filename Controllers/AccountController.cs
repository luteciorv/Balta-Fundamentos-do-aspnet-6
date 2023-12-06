using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("v1/accounts")]
public class AccountController : ControllerBase
{
    [HttpPost]
    public IActionResult Login([FromServices] TokenService tokenService)
    {
        var token = tokenService.GenerateToken(null);
        return Ok(token);
    }

    [Authorize(Roles = "user")]
    [HttpGet("user")]
    public IActionResult GetUser()
        => Ok(User.Identity?.Name);

    [Authorize(Roles = "author")]
    [HttpGet("author")]
    public IActionResult GetAuthor()
        => Ok(User.Identity?.Name);

    [Authorize(Roles = "admin")]
    [HttpGet("admin")]
    public IActionResult GetAdmin()
        => Ok(User.Identity?.Name);
}
