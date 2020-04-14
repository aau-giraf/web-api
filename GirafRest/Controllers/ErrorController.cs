﻿using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Controllers
{
    [Route("v1/[controller]")]
    public class ErrorController : Controller
    {
        /// <summary>
        /// All Error requests will redirect to this endpoint
        /// </summary>
        /// <returns>ErrorCode.NotFound</returns>
        [HttpGet(""), HttpPost(""), HttpPut(""), HttpDelete("")]
        public ActionResult<Response> Index()
        {
            Response.StatusCode = 200;

            return NotFound(new ErrorResponse(ErrorCode.NotFound));
        }
    }
}