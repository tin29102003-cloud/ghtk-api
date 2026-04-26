using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAuthertication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientSourceController : ControllerBase
    {
        private readonly IClientSourceAuthenticationHandler handler;
        public ClientSourceController(IClientSourceAuthenticationHandler handler) {
            this.handler = handler;
        }
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (handler.Validate(id))
            {
                return Ok();
            }
            return Unauthorized();
        }
    }
}
