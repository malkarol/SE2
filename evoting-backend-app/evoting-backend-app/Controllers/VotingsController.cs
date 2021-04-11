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
    public class VotingsController : Controller
    {
        private readonly VotingsService votingsService;

        public VotingsController(VotingsService votingsService)
        {
            this.votingsService = votingsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voting>>> Get()
        {
            return new ObjectResult(await votingsService.GetAllVotings());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Voting>> Get(string id)
        {
            var voting = await votingsService.GetVoting(id);
            if (voting == null)
                return new NotFoundResult();

            return new ObjectResult(voting);
        }

        [SwaggerOperation(Summary = "(Id field will be ignored, no need to pass it)")]
        [HttpPost]
        public async Task<ActionResult<Voting>> Post([FromBody] Voting voting)
        {
            await votingsService.CreateVoting(voting);
            return new OkObjectResult(voting);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Voting>> Put(string id, [FromBody] Voting voting)
        {
            var votingFromDb = await votingsService.GetVoting(id);
            if (votingFromDb == null)
                return new NotFoundResult();
            voting.Id = votingFromDb.Id;
            await votingsService.UpdateVoting(voting);
            return new OkObjectResult(voting);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var votingFromDb = await votingsService.GetVoting(id);
            if (votingFromDb == null)
                return new NotFoundResult();
            await votingsService.DeleteVoting(id);
            return new OkResult();
        }

    }
}
