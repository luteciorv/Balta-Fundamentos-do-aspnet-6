using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
[Route("v1/posts")]
public class PostController : ControllerBase
{
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 25)
    {
        try
        {
            var count = await context.Posts.AsNoTracking().CountAsync();
            var posts = await context.Posts
                            .AsNoTracking()
                            .Include(p => p.Category)
                            .Include(p => p.Author)
                            .Select(p => new ListPostsViewModel
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Slug = p.Slug,
                                LastUpdateDate = p.LastUpdateDate,
                                Category = p.Category.Name,
                                Author = $"{p.Author.Name} ({p.Author.Email})"
                            })
                            .Skip(page * pageSize)
                            .Take(pageSize)
                            .OrderByDescending(p => p.LastUpdateDate)
                            .ToListAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                page,
                pageSize,
                posts
            }));
        }
        catch(Exception ex)
        {
            return StatusCode(500, new ResultViewModel<List<Post>>($"Não foi possível recuperar os posts do banco de dados. Erro: {ex.Message}"));
        }
    }
        
}
