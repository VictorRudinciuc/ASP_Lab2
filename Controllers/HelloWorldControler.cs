using Microsoft.AspNetCore.Mvc;

namespace projectAsp.Controllers
{
    [ApiController]
    [Route("api")]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet("hello-world")]
        public IActionResult HelloWorld()
        {
            return Ok(new { message = "Hello World from ASP.NET Core!" });
        }
    }
}