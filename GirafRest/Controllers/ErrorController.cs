using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Controllers
{
    [Route("v1/[controller]")]
    public class ErrorController : Controller
    {
        [HttpGet(""), HttpPost(""), HttpPut(""), HttpDelete("")]
        public Response Index([FromQuery] int status = 400)
        {
            Response.StatusCode = 200;

            return new ErrorResponse(ErrorCode.NotFound);
        }
    }
}