﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models.Responses;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Status-endpoint; Getting status of HTTP, DB etc, for clients to see status
    /// </summary>
    [Route("v1/[controller]")]
    public class StatusController : Controller
    {
        private readonly IGirafService _giraf;

        /// <summary>
        /// Constructor for StatusController
        /// </summary>
        /// <param name="giraf">Service Injection</param>
        public StatusController(IGirafService giraf)
        {
            _giraf = giraf;
        }

        /// <summary>
        /// End-point for checking if the API is running
        /// </summary>
        /// <returns>Success Reponse.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        public ActionResult Status()
        {
            Console.WriteLine("return ok on status");
            return Ok(new SuccessResponse("GIRAF API is running!"));
        }

        /// <summary>
        /// End-point for checking connection to the database
        /// </summary>
        /// <returns>Success response if connection to database else ErrorResponse</returns>
        [HttpGet("database")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public ActionResult DatabaseStatus()
        {
            try
            {
                _giraf._context.Users.FirstOrDefault();
                return Ok(new SuccessResponse("Connection to database"));
            }
            catch (System.Exception e)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ErrorResponse(ErrorCode.Error, "Error when connecting to database", e.Message));
            }
        }

        /// <summary>
        /// Endpoint for getting git version info i.e. branch and commithash 
        /// </summary>
        /// <returns>branch and commit hash for this API instance</returns>
        [HttpGet("version-info")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        public ActionResult GetVersionInfo()
        {
            try
            {
                // Get the solution folder
                var gitpath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

                // Check if it is in a build and not server
                if (gitpath.Contains("bin"))
                {
                    gitpath = Path.GetFullPath(Path.Combine(gitpath, @"..\..\..\"));
                }

                // Get the hidden .git folder
                gitpath += ".git/";

                // Get commit hash from the HEAD file in the .git folder.
                var commitHash = System.IO.File.ReadLines(gitpath + "HEAD").First();
                
                // Return the response
                return Ok(new SuccessResponse($"CommitHash: {commitHash}"));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.Error, $"Exception: " + e.ToString()));
            }
        }
    }
}
