﻿using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromServices] IMemoryCache cache)
        {
            try
            {
                var categories = await cache.GetOrCreateAsync("CategoriesCache", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return await context.Categories.AsNoTracking().ToListAsync();
                });

                return Ok(new ResultViewModel<IEnumerable<Category>?>(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<IEnumerable<Category>>($"Não foi possível recuperar as categorias. Erro: {ex.Message}"));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context, 
            [FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                if (category is null) return NotFound(new ResultViewModel<Category>($"A categoria de id {id} não foi encontrada."));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>($"Não foi possível recuperar a categoria de id {id}. Erro: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            var category = new Category
            {
                Id = 0,
                Name = viewModel.Name,
                Slug = viewModel.Slug.ToLower()
            };

            try
            {
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>($"Não foi possível criar a categoria. Erro: {ex.Message}"));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

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
                return StatusCode(500, new ResultViewModel<Category>($"Não foi possível atualizar a categoria de id {id}. Erro: {ex.Message}"));
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
                return StatusCode(500, new ResultViewModel<Category>($"Não foi possível apagar a categoria de id {id}. Erro: {ex.Message}"));
            }
        }
    }
}
