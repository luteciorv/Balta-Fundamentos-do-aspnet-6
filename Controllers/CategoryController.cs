using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
            => Ok(await context.Categories.AsNoTracking().ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (category is null) return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] Category model)
        {
            await context.Categories.AddAsync(model);
            await context.SaveChangesAsync();

            return Created($"{model.Id}", model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id,
            [FromBody] Category model)
        {
            var modelToUpdate = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (modelToUpdate is null) return NotFound();

            modelToUpdate.Name = model.Name;
            modelToUpdate.Slug = model.Slug;

            context.Categories.Update(modelToUpdate);
            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            var model = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (model is null) return NotFound();

            context.Categories.Remove(model);
            await context.SaveChangesAsync();
            
            return Ok();
        }
    }
}
