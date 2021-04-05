using evoting_backend_app.Models;
using evoting_backend_app.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace evoting_backend_app.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class VotersController : Controller
    {
        private readonly VotersService votersService;

        public VotersController(VotersService votersService)
        {
            this.votersService = votersService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voter>>> Get()
        {
            return new ObjectResult(await votersService.GetAllVoters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Voter>> Get(string id)
        {
            var user = await votersService.GetVoter(id);
            if (user == null)
                return new NotFoundResult();

            return new ObjectResult(user);
        }

        [SwaggerOperation(Summary = "(Id field will be ignored, don't need to pass it")]
        [HttpPost]
        public async Task<ActionResult<Voter>> Post([FromBody] Voter voter)
        {
            await votersService.CreateVoter(voter);
            return new OkObjectResult(voter);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Voter>> Put(string id, [FromBody] Voter voter)
        {
            var voterFromDb = await votersService.GetVoter(id);
            if (voterFromDb == null)
                return new NotFoundResult();
            voter.Id = voterFromDb.Id;
            await votersService.UpdateVoter(voter);
            return new OkObjectResult(voter);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var voterFromDb = await votersService.GetVoter(id);
            if (voterFromDb == null)
                return new NotFoundResult();
            await votersService.DeleteVoter(id);
            return new OkResult();
        }
    }
}
