using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Api.Servives;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("v1/account")]
    public class UserController : ControllerBase
    {

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices] DataContext context, [FromBody] User model)
        {
            try
            {

                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == model.UserName.ToLower() && x.Password == model.Password);

                if (user == null) return NotFound(new { message = "Usuário ou senha inválidos" });

                var token = TokenService.GenerateToken(user);
                user.Password = "";

                return new
                {
                    user = user,
                    token = token
                };
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível efetuar login" });
            }
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            try
            {
                var users = await context.Users.AsNoTracking().ToListAsync();
                users.ForEach(x => x.Password = string.Empty);
                return users;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter os usuários" });
            }

        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "manager, employee")]
        public async Task<ActionResult<User>> GetById([FromServices] DataContext context, int id)
        {
            try
            {
                var User = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                User.Password = "";
                return User;

            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível obter o usuário" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Create([FromServices] DataContext context, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (string.IsNullOrEmpty(user.Role)) user.Role = "employee";
                
                context.Users.Add(user);
                await context.SaveChangesAsync();
                user.Password = "";
                return user;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put([FromServices] DataContext context, int id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                if (id != user.Id)
                    return NotFound(new { message = "Usuário não encontrado" });

                context.Entry(user).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return user;
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível Alterar o usuário" });

            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Delete([FromServices] DataContext context, int id)
        {
            try
            {
                var User = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (User != null)
                {
                    context.Users.Remove(User);
                    await context.SaveChangesAsync();
                    return User;
                }
                else return NotFound(new { message = "Usuário não encontrado" });
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possível deletar o usuário" });

            }

        }
        
        [HttpGet]
        [Route("Anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anônimo";

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated() => String.Format("Autenticado - {0}", User.Identity.Name);

        [HttpGet]
        [Route("employee")]
        [Authorize(Roles = "employee, manager")]
        public string Employee() => "Funcionario";

        [HttpGet]
        [Route("manager")]
        [Authorize(Roles = "manager")]
        public string Manager() => "Gerente";
    }
}