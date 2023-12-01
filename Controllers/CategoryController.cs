using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromServices] BlogDataContext context)
            => Ok(context.Categories.AsNoTracking().ToList());
    }
}
