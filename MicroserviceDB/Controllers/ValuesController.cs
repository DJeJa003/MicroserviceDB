using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MicroserviceDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            try
            {
                // Process the received value (you can save it to the database, perform some operation, etc.)
                _logger.LogInformation($"Received value: {value}");

                // Return a simple response
                return Ok($"Received value: {value}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing POST request: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
