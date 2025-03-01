using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.Lambda.Core;


namespace GoOrder.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            return Ok(new { OrderId = id, Status = "Processed" });
            // CI/CD trigger
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] object order)
        {
            return Created("", new { Message = "Order Created" });
        }

    }
}
