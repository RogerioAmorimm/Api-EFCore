using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace CrudEf.Controllers
{
    [ApiController]
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            try
            {
                var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();

                return products;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter os produtos" });
            }

        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Product>> GetById([FromServices] DataContext context, int id)
        {
            try
            {
                var product = await context.Products.Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

                return product;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter o produto" });
            }

        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {
            try
            {
                var product = await context.Products
                    .Include(x => x.Category)
                    .AsNoTracking()
                    .Where(x => x.Category.Id == id)
                    .ToListAsync();

                return product;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter os produtos pela categoria" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "manager, employee")]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Product>> Post([FromServices] DataContext context, [FromBody] ProductDTO model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                Product product = await ExtractProduct(context, model);

                context.Products.Add(product);
                await context.SaveChangesAsync();
                return product;

            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o produto " });
            }
        }

        private async Task<Product> ExtractProduct(DataContext context, ProductDTO model)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == model.CategoryID);
            var product = new Product()
            {
                Id = model.Id,
                Price = model.Price,
                Title = model.Title,
                Description = model.Description,
                Category = category
            };
            return product;
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager, employee")]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Product>> Put([FromServices] DataContext context, [FromBody] ProductDTO model, int id)
        {
            try
            {

                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (id != model.Id) return NotFound(new { message = "Produto não encontrado" });

                var productContext = await this.ExtractProduct(context, model);

                context.Entry(productContext).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return productContext;

            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível alterar o produto" });
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        [ResponseCache(VaryByHeader = "User-Agent",Location = ResponseCacheLocation.Any, Duration = 30 )]
        public async Task<ActionResult<Product>> Delete([FromServices] DataContext context, int id)
        {
            try
            {
                var product = await context.Products.Include(x=>x.Category).FirstOrDefaultAsync(x => x.Id == id);

                if (product != null)
                {
                    context.Products.Remove(product);
                    await context.SaveChangesAsync();
                    return product;
                }
                else return NotFound(new {message = "Produto não encontrado"});
            }
            catch (System.Exception)
            {
                return BadRequest(new {message = "Não foi possível deletar o produto"});
            }

        }
    }
}