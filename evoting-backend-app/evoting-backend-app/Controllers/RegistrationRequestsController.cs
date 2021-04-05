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
    public class RegistrationRequestsController : Controller
    {
        private readonly RegistrationRequestsService registrationRequestsService;

        public RegistrationRequestsController(RegistrationRequestsService registrationRequestsService)
        {
            this.registrationRequestsService = registrationRequestsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistrationRequest>>> Get()
        {
            return new ObjectResult(await registrationRequestsService.GetAllRegistrationRequests());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistrationRequest>> Get(string id)
        {
            var registrationRequest = await registrationRequestsService.GetRegistrationRequest(id);
            if (registrationRequest == null)
                return new NotFoundResult();

            return new ObjectResult(registrationRequest);
        }

        [SwaggerOperation(Summary = "(Id field will be ignored, don't need to pass it")]
        [HttpPost]
        public async Task<ActionResult<RegistrationRequest>> Post([FromBody] RegistrationRequest registrationRequest)
        {
            await registrationRequestsService.CreateRegistrationRequest(registrationRequest);
            return new OkObjectResult(registrationRequest);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RegistrationRequest>> Put(string id, [FromBody] RegistrationRequest registrationRequest)
        {
            var registrationRequestFromDb = await registrationRequestsService.GetRegistrationRequest(id);
            if (registrationRequestFromDb == null)
                return new NotFoundResult();
            registrationRequest.Id = registrationRequestFromDb.Id;
            await registrationRequestsService.UpdateRegistrationRequest(registrationRequest);
            return new OkObjectResult(registrationRequest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var registrationRequestFromDb = await registrationRequestsService.GetRegistrationRequest(id);
            if (registrationRequestFromDb == null)
                return new NotFoundResult();
            await registrationRequestsService.DeleteRegistrationRequest(id);
            return new OkResult();
        }

    }
}
