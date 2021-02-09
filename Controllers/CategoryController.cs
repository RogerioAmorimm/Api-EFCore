
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            try
            {
                var categories = await context.Categories.AsNoTracking().ToListAsync();
                return categories;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter as categorias" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Category>> Get([FromServices] DataContext context, int id)
        {
            try
            {
                var category = await context.Categories
                       .AsNoTracking()
                       .FirstOrDefaultAsync(x => x.Id == id);

                return category;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter a categoria" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "manager, employee")]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Category>> Post([FromServices] DataContext context, [FromBody] Category model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível criar a categoria " });
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager, employee")]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Category>> Put([FromServices] DataContext context, [FromBody] Category category, int id)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (id != category.Id) return NotFound(new { message = "Categoria não encontrada" });

                context.Entry(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return category;

            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível alterar a categoria" });
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Category>> Delete([FromServices] DataContext context, int id)
        {
            try
            {
                var Category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (Category == null) return NotFound(new { message = "Categoria não encontrada" });

                await context.Products.Where(x => x.Category.Id == id).ForEachAsync(x => x.Category = null);
                context.Categories.Remove(Category);
                await context.SaveChangesAsync();

                return Category;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível deletar o categoria"});
            }

        }

    }
}