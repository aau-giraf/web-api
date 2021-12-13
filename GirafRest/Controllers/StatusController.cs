using System;
using System.IO;
using System.Linq;
using GirafRest.Models.Responses;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GirafRest.Data;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Status-endpoint; Getting status of HTTP, DB etc, for clients to see status
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class StatusController : Controller
    {
        private readonly IGirafService _giraf;

        // SHOULD BE REMOVED AFTER REFACTORING OF THIS CONTROLLER HAS BEEN COMPLETED!
        private readonly GirafDbContext _context;

        /// <summary>
        /// Constructor for StatusController
        /// </summary>
        /// <param name="giraf">Service Injection</param>
        public StatusController(IGirafService giraf, GirafDbContext context)
        {
            _giraf = giraf;
            _context = context;
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
                _context.Users.FirstOrDefault();
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
                // Get the hidden .git folder
                var gitpath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/.git/";

                // Get commit hash from the HEAD file in the .git folder.
                var commitHash = System.IO.File.ReadLines(gitpath + "HEAD").First();

                // Return the response
                return Ok(new SuccessResponse($"CommitHash: {commitHash}"));


                // Previously, we retrieved the refs/head/branch, and then retrieved the commit hash.
                // As the "HEAD" file now ONLY contains the commmit hash, this is no longer possible.
                // This code is preserved in case future students wants to implement the behavior again.
                /*
                var pathToHead = System.IO.File.ReadLines(gitpath + "HEAD").First().Split(" ").Last();

                var hash = System.IO.File.ReadLines(gitpath + pathToHead).First();
                // this assumes that branches are not named with / however this should be enforced anyways
                var branch = pathToHead.Split("/").Last();
                return Ok(new SuccessResponse($"Branch: {branch} CommitHash: {hash}"));
                */
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.Error, $"Exception: " + e.ToString()));
            }
        }
    }
}
