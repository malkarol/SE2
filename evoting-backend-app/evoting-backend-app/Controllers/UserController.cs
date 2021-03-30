using evoting_backend_app.Models;
using evoting_backend_app.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evoting_backend_app.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }


        // GET api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return new ObjectResult(await _userService.GetAllUsers());
        }

        // GET api/user/1
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(long id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
                return new NotFoundResult();

            return new ObjectResult(user);
        }

        // POST api/user
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        {
            user.Id = (await _userService.GetNextUserId()); //.ToString();
            await _userService.CreateUser(user);
            return new OkObjectResult(user);
        }

        // PUT api/user/1
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put(long id, [FromBody] User user)
        {
            var userFromDb = await _userService.GetUser(id);
            if (userFromDb == null)
                return new NotFoundResult();
            user.Id = userFromDb.Id;
            await _userService.UpdateUser(user);
            return new OkObjectResult(user);
        }

        // DELETE api/user/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var post = await _userService.GetUser(id);
            if (post == null)
                return new NotFoundResult();
            await _userService.DeleteUser(id);
            return new OkResult();
        }
    }
}
