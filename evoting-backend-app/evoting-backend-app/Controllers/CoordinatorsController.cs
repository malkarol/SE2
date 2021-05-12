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
    public class CoordinatorsController : Controller
    {
        private readonly CoordinatorsService coordinatorsService;

        public CoordinatorsController(CoordinatorsService coordinatorsService)
        {
            this.coordinatorsService = coordinatorsService;
        }

        // ---

        public class Coordinator_BasicInfo_QueryParameters : QueryParameters
        {
            public CoordinatorType? Type { get; set; }
            public String FirstName { get; set; }
            public String LastName { get; set; }        
            public String Email { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Coordinator_BasicInfo_DTO>>> GetAllCoordinators([FromQuery] Coordinator_BasicInfo_QueryParameters queryParameters)
        {
            var coordinators = await coordinatorsService.GetAllCoordinators(queryParameters);
            if (coordinators == null)
                return new NotFoundResult();

            return new ObjectResult(coordinators);
        }

        [HttpGet("{coordinatorId}")]
        public async Task<ActionResult<Coordinator_BasicInfo_DTO>> GetCoordinator(string coordinatorId)
        {
            var coordinator = await coordinatorsService.GetCoordinator(coordinatorId);
            if (coordinator == null)
                return new NotFoundResult();

            return new ObjectResult(coordinator);
        }

        [HttpPatch("{coordinatorId}")]
        public async Task<ActionResult<Coordinator_BasicInfo_DTO>> UpdateCoordinator(string coordinatorId, [FromBody] Coordinator_Update_DTO coordinatorUpdateData)
        {
            var coordinator = await coordinatorsService.UpdateCoordinator(coordinatorId, coordinatorUpdateData);
            if (coordinator == null)
                return new NotFoundResult();

            return new ObjectResult(coordinator);
        }

        [HttpPost]
        public async Task<ActionResult<Coordinator_BasicInfo_DTO>> AddCoordinator([FromBody] Coordinator_Add_DTO coordinatorAddData)
        {
            var coordinator = await coordinatorsService.AddCoordinator(coordinatorAddData);
            return new ObjectResult(coordinator);
        }

        public class CoordinatorVoting_BasicInfo_QueryParameters : QueryParameters
        {
            public string VotingName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool? Active { get; set; }

            //public String SortBy { get; set; }
        }


        [HttpGet("{coordinatorId}/Votings")]
        public async Task<ActionResult<PagedList<CoordinatorVoting_BasicInfo_DTO>>> GetCoordinatorVotings(string coordinatorId, [FromQuery] CoordinatorVoting_BasicInfo_QueryParameters queryParameters)
        {
            var votings = await coordinatorsService.GetCoordinatorVotings(coordinatorId, queryParameters);
            if (votings == null)
                return new NotFoundResult();

            return new ObjectResult(votings);
        }

    }
}
