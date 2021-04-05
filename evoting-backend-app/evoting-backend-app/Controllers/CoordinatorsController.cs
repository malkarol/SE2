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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coordinator>>> Get()
        {
            return new ObjectResult(await coordinatorsService.GetAllCoordinators());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coordinator>> Get(string id)
        {
            var user = await coordinatorsService.GetCoordinator(id);
            if (user == null)
                return new NotFoundResult();

            return new ObjectResult(user);
        }

        [SwaggerOperation(Summary = "(Id field will be ignored, don't need to pass it")]
        [HttpPost]
        public async Task<ActionResult<Coordinator>> Post([FromBody] Coordinator coordinator)
        {
            await coordinatorsService.CreateCoordinator(coordinator);
            return new OkObjectResult(coordinator);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Coordinator>> Put(string id, [FromBody] Coordinator coordinator)
        {
            var coordinatorFromDb = await coordinatorsService.GetCoordinator(id);
            if (coordinatorFromDb == null)
                return new NotFoundResult();
            coordinator.Id = coordinatorFromDb.Id;
            await coordinatorsService.UpdateCoordinator(coordinator);
            return new OkObjectResult(coordinator);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var coordinatorFromDb = await coordinatorsService.GetCoordinator(id);
            if (coordinatorFromDb == null)
                return new NotFoundResult();
            await coordinatorsService.DeleteCoordinator(id);
            return new OkResult();
        }
    }
}
