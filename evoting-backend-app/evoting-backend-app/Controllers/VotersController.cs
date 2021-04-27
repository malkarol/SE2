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

        [HttpGet("{id}")]
        public async Task<ActionResult<Voter_BasicInfo_DTO>> GetVoter(string id)
        {
            var voter = await votersService.GetVoter(id);
            if (voter == null)
                return new NotFoundResult();

            return new ObjectResult(voter);
        }
    }
}
