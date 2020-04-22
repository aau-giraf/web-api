using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Handles errors
    /// </summary>
    [Route("v1/[controller]")]
    public class ErrorController : Controller
    {
        /// <summary>
        /// All Error requests will redirect to this endpoint
        /// </summary>
        /// <returns>ErrorCode.NotFound</returns>
        [HttpGet(""), HttpPost(""), HttpPut(""), HttpDelete(""), HttpPatch("")]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public ActionResult Index()
        {
            return NotFound(new ErrorResponse(ErrorCode.NotFound, "The endpoint could not be found"));
        }
    }
}