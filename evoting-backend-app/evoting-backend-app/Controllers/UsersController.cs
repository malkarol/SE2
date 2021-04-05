using evoting_backend_app.Models;
using evoting_backend_app.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evoting_backend_app.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class UsersController : Controller
    {
        private readonly UsersService _usersService;

        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }


        // GET api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return new ObjectResult(await _usersService.GetAllUsers());
        }

        // GET api/user/1
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(string id)
        {
            var user = await _usersService.GetUser(id);
            if (user == null)
                return new NotFoundResult();

            return new ObjectResult(user);
        }

        // POST api/user
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] UserDTO_Post user)
        {
            await _usersService.CreateUser(user);
            return new OkObjectResult(user);
        }

        // PUT api/user/1
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put(string id, [FromBody] User user)
        {
            var userFromDb = await _usersService.GetUser(id);
            if (userFromDb == null)
                return new NotFoundResult();
            user.Id = userFromDb.Id;
            await _usersService.UpdateUser(user);
            return new OkObjectResult(user);
        }

        // DELETE api/user/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var post = await _usersService.GetUser(id);
            if (post == null)
                return new NotFoundResult();
            await _usersService.DeleteUser(id);
            return new OkResult();
        }
    }
}
