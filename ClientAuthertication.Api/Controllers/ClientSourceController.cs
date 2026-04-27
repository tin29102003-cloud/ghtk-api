using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAuthertication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientSourceController : ControllerBase
    {
        private readonly ILogger<ClientSourceController> logger;
        private readonly IClientSourceAuthenticationHandler handler;
        public ClientSourceController(IClientSourceAuthenticationHandler handler, ILogger<ClientSourceController> logger) {
            this.handler = handler;
            this.logger = logger;
        }
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            logger.LogInformation("Authemtocating client it: {Id}", id);
            if (handler.Validate(id))
            {
                logger.LogInformation("Client {Id} authenticated successfully", id);
                return Ok();
            }
            logger.LogWarning("Client {Id} failed authentication", id);
            return Unauthorized();
        }
    }
}
