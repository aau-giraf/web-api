using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Handles errors
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class ErrorController : Controller
    {
        /// <summary>
        /// This endpoint is reached when an error happens in the routing
        /// </summary>
        /// <param name="statusCode">The statuscode gotten when the error happened</param>
        [AllowAnonymous]
        [AcceptVerbs("POST", "GET", "PUT", "DELETE", "PATCH")]
        public ActionResult StatusCodeEndpoint([FromQuery] int statusCode)
        {
               
            var statusCodeReExecuteFeature = HttpContext.
                Features.
                Get<IStatusCodeReExecuteFeature>();

            var OriginalURL = "";

            if (statusCodeReExecuteFeature != null)
            {
                OriginalURL =
                    statusCodeReExecuteFeature.OriginalPathBase
                    + statusCodeReExecuteFeature.OriginalPath
                    + statusCodeReExecuteFeature.OriginalQueryString;
            }

            switch (statusCode)
            {
                case StatusCodes.Status401Unauthorized:
                    return Unauthorized(new ErrorResponse(ErrorCode.NotAuthorized, "Unauthorized"));
                case StatusCodes.Status403Forbidden:
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.Forbidden, "User does not have permission"));
                case StatusCodes.Status404NotFound:
                    return NotFound(new ErrorResponse(ErrorCode.NotFound, "Not found", "You tried to reach " + Request.Method + ": " + OriginalURL));
                case 0:
                    return BadRequest(new ErrorResponse(ErrorCode.UnknownError, "Bad Request"));
                default:
                    return StatusCode(statusCode, new ErrorResponse(ErrorCode.UnknownError, "Statuscode: " + statusCode));
            }

        }
    }
}
