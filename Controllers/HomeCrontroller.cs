using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Config([FromServices]DataContext context)
        {
            try
            {

                context.Users.Add(new User{Id = 1, UserName = "admin", Password = "admin", Role = "manager"});
                await context.SaveChangesAsync();

                return Ok(new {message = "Aplicação Configurada"});

            }
            catch (System.Exception)
            {
                return  BadRequest(new {message = "Não foi possível configurar aplicação"});
            }
        }
    }
}