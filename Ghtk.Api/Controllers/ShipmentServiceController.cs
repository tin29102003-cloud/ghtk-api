using Ghtk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ghtk.Api.Controllers
{
    [Route("/services/shipment")]
    public class ShipmentServiceController : ControllerBase
    {
      public ShipmentServiceController()
      {
        }
        [HttpPost]
        [Route("order")]
        [Authorize]
        public IActionResult CreatedOrder([FromBody] CreateOrder shipment)
        {
            return Ok();
        }
    }
}
