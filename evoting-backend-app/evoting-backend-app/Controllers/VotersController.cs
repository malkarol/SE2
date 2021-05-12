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

        // ---

        public class Voter_BasicInfo_QueryParameters : QueryParameters
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            //public uint MinYearOfBirth { get; set; }
            //public uint MaxYearOfBirth { get; set; } = (uint)DateTime.Now.Year;
            //public bool ValidYearRange => MaxYearOfBirth > MinYearOfBirth;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Voter>>> GetAllVoters([FromQuery] Voter_BasicInfo_QueryParameters queryParameters)
        {
            var voters = await votersService.GetAllVoters(queryParameters);
            if (voters == null)
                return new NotFoundResult();

            return new ObjectResult(voters);
        }

        [HttpGet("{voterId}")]
        public async Task<ActionResult<Voter_BasicInfo_DTO>> GetVoter(string voterId)
        {
            var voter = await votersService.GetVoter(voterId);
            if (voter == null)
                return new NotFoundResult();

            return new ObjectResult(voter);
        }

        public class Voter_Voting_BasicInfo_QueryParameters : QueryParameters
        {
            public string Name { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool? Active { get; set; }
            public RegistrationRequestStatus? RegistrationStatus { get; set; }
            public bool? AlreadyVoted { get; set; }

            //public String SortBy { get; set; }
        }

        [HttpGet("{voterId}/Votings")]
        public async Task<ActionResult<PagedList<VoterVoting_BasicInfo_DTO>>> GetVoterVotings(string voterId, [FromQuery] Voter_Voting_BasicInfo_QueryParameters queryParameters)
        {
            var votings = await votersService.GetVoterVotings(voterId, queryParameters);
            if (votings == null)
                return new NotFoundResult();

            return new ObjectResult(votings);
        }



        [HttpPatch("{id}")]
        public async Task<ActionResult<Voter_BasicInfo_DTO>> UpdateVoter(string voterId, [FromBody] Voter_Update_DTO voterUpdateData)
        {
            var voter = await votersService.UpdateVoter(voterId, voterUpdateData);
            if (voter == null)
                return new NotFoundResult();

            return new ObjectResult(voter);
        }

        [HttpPost]
        public async Task<ActionResult<Voter_BasicInfo_DTO>> AddVoter([FromBody] Voter_Add_DTO voterAddData)
        {
            var voter = await votersService.AddVoter(voterAddData);
            return new ObjectResult(voter);
        }
    }
}
