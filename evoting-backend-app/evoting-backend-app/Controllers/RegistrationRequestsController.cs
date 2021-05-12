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
    [Route("api/Votings/")]
    public class RegistrationRequestsController : Controller
    {
        private readonly RegistrationRequestsService registrationRequestsService;

        public RegistrationRequestsController(RegistrationRequestsService registrationRequestsService)
        {
            this.registrationRequestsService = registrationRequestsService;
        }

        // ---

        [HttpPost("{votingId}/RegistrationRequests")]
        public async Task<IActionResult> AddRegistrationRequest(string votingId, [FromBody] RegistrationRequest_Add_DTO registrationRequestAddData)
        {
            var result = await registrationRequestsService.AddRegistrationRequest(votingId, registrationRequestAddData);
            return new OkResult();
        }

        [HttpPost("{votingId}/RegistrationRequests/{registrationRequestId}/Decide")]
        public async Task<IActionResult> RegistrationRequestDecide(string votingId, string registrationRequestId, [FromBody] RegistrationRequest_Decision_DTO registrationRequestDecisionData)
        {
            var result = await registrationRequestsService.RegistrationRequestDecide(votingId, registrationRequestId, registrationRequestDecisionData);
            return new OkResult();
        }

        public class RegistrationRequest_BasicInfo_QueryParameters : QueryParameters
        {
            public RegistrationRequestStatus? Status { get; set; }

            //public String SortBy { get; set; }
        }

        [HttpGet("{votingId}/RegistrationRequests")]
        public async Task<ActionResult<PagedList<RegistrationRequest_BasicInfo_DTO>>> GetVotingRegistrationRequests(string votingId, [FromQuery] RegistrationRequest_BasicInfo_QueryParameters queryParameters)
        {
            var registrationRequests = await registrationRequestsService.GetVotingRegistrationRequests(votingId, queryParameters);
            if (registrationRequests == null)
                return new NotFoundResult();

            return new ObjectResult(registrationRequests);
        }

        [HttpGet("{votingId}/RegistrationRequests/{registrationRequestId}")]
        public async Task<ActionResult<RegistrationRequest>> GetVotingRegistrationRequest(string votingId, string registrationRequestId)
        {
            var registrationRequest = await registrationRequestsService.GetRegistrationRequest(votingId, registrationRequestId);
            if (registrationRequest == null)
                return new NotFoundResult();

            return new ObjectResult(registrationRequest);
        }
    }
}
