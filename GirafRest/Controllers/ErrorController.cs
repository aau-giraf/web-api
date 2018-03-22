using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Controllers
{
    [Route("v1/[controller]")]
    public class ErrorController : Controller
    {
        [HttpGet(""), HttpPost("")]
        public Response Index([FromQuery] int status = 400)
        {
            Response.StatusCode = 200;

            if (status == 401)
            {
                return new ErrorResponse(ErrorCode.NotAuthorized);
            }

            return new ErrorResponse(ErrorCode.UnkonwnError);
        }
    }
}