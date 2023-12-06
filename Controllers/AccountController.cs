using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureIdentity.Password;

namespace Blog.Controllers;

[ApiController]
[Route("v1/accounts")]
public class AccountController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromServices] BlogDataContext context,
        [FromBody] RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid) 
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = new User
        {
            Name = viewModel.Name,
            Email = viewModel.Email,
            Slug = viewModel.Email.Replace('@', '-').Replace('.', '-')
        };

        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email, 
                password
            }));
        }
        catch(Exception ex)
        {
            return StatusCode(500, new ResultViewModel<dynamic>($"Não foi possível registrar o usuário. Erro: {ex.Message}"));
        }
    }

    [HttpPost("login")]
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
