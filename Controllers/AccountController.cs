using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;

[ApiController]
[Route("v1/accounts")]
public class AccountController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromServices] BlogDataContext context,
        [FromServices] EmailService emailService,
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

            emailService.Send(
                user.Name, 
                user.Email, 
                "Bem vindo ao Blog!", 
                $"A sua senha é: <strong>{password}</strong>"
            );

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
    public async Task<IActionResult> LoginAsync(
        [FromServices] TokenService tokenService,
        [FromServices] BlogDataContext context,
        [FromBody] LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context.Users
                            .AsNoTracking()
                            .Include(u => u.Roles)
                            .FirstOrDefaultAsync(u => u.Email == viewModel.Email);
        if (user is null)
            return StatusCode(404, new ResultViewModel<string>($"Usuário ou senha inválido"));

        if(!PasswordHasher.Verify(user.PasswordHash, viewModel.Password))
            return StatusCode(404, new ResultViewModel<string>($"Usuário ou senha inválido"));

        try
        {
            var token = tokenService.GenerateToken(user); 
            return Ok(new ResultViewModel<string>(token));
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Não foi possível logar o usuário. Erro: {ex.Message}");
        }
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
