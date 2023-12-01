using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
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
        public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel viewModel)
        {
            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = viewModel.Name,
                    Slug = viewModel.Slug.ToLower()
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"{category.Id}", category);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Não foi possível criar a categoria. Exceção: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel viewModel)
        {
            var model = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (model is null) return NotFound();

            model.Name = viewModel.Name;
            model.Slug = viewModel.Slug.ToLower();

            try
            {
                context.Categories.Update(model);
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Não foi possível atualizar a categoria. Exceção: {ex.Message}");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            var model = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (model is null) return NotFound();

            try
            {
                context.Categories.Remove(model);
                await context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Não foi possível remover a categoria. Exceção: {ex.Message}");
            }
        }
    }
}
