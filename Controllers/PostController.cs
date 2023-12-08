using Blog.Data;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1/posts")]
    public class PostController : ControllerBase
    {
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
            => Ok(await context.Posts
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
                            .ToListAsync()
                    );
            
    }
}
