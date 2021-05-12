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

        // ---

        public class Voting_BasicInfo_QueryParameters : QueryParameters
        {
            public String Name { get; set; }
            public DateTime? RangeStart { get; set; }
            public DateTime? RangeEnd { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Voting_BasicInfo_DTO>>> GetAllVotings([FromQuery] Voting_BasicInfo_QueryParameters queryParameters)
        {
            var voters = await votingsService.GetAllVotings(queryParameters);
            return new ObjectResult(voters);
        }

        [HttpGet("{votingId}")]
        public async Task<ActionResult<Voting>> GetVoting(string votingId)
        {
            var voting = await votingsService.GetVoting(votingId);
            if (voting == null)
                return new NotFoundResult();

            return new ObjectResult(voting);
        }

        [HttpGet("{votingId}/Voter/{voterId}")]
        public async Task<ActionResult<Voting_Voter_DTO>> GetVotingVoter(string votingId, string voterId)
        {
            var voting = await votingsService.GetVotingVoter(votingId, voterId);
            if (voting == null)
                return new NotFoundResult();

            return new ObjectResult(voting);
        }

        [HttpPost]
        public async Task<ActionResult<Voting>> AddVoting([FromBody] Voting_Add_DTO votingAddData)
        {
            var voting = await votingsService.AddVoting(votingAddData);
            return new OkObjectResult(voting);
        }

        [HttpPost("{votingId}/Vote")]
        public async Task<IActionResult> Vote(string votingId, [FromBody] Vote_Ballot_DTO voteBallot)
        {
            await votingsService.Vote(votingId, voteBallot);
            return new OkResult();
        }

        [HttpPatch("{votingId}")]
        public async Task<ActionResult<Voting>> UpdateVoting(string votingId, [FromBody] Voting_Update_DTO votingUpdateData)
        {
            var voting = await votingsService.UpdateVoting(votingId, votingUpdateData);
            if (voting == null)
                return new NotFoundResult();

            return new ObjectResult(voting);
        }

        [HttpPost("{votingId}/AddCoordinator")]
        public async Task<IActionResult> AddCoordinatorToVoting(string votingId, [FromBody] Voting_AddCoordinator_DTO votingAddCoordinatorData)
        {
            await votingsService.AddCoordinatorToVoting(votingId, votingAddCoordinatorData);
            return new OkResult();
        }


    }
}
