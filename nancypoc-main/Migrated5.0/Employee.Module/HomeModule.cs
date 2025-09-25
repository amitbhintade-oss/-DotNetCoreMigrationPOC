using Microsoft.AspNetCore.Mvc;

namespace Employee.Host.Modules
{
    [ApiController]
    [Route("")]
    public class HomeModule : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Api-Employee");
        }
    }
}
