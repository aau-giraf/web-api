using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Serves no purpose apart from testing.
    /// </summary>
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        private string[] values = { "Hest", "Mel", "Lotte" };

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(values);
        }
    }
}
