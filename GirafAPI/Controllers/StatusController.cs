﻿using GirafEntities.Responses;
using GirafRepositories.Persistence;
using GirafServices.User;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using GirafRepositories.Interfaces;

namespace GirafAPI.Controllers
{
    /// <summary>
    /// Status-endpoint; Getting status of HTTP, DB etc, for clients to see status
    /// </summary>
    [Route("v1/[controller]")]
    public class StatusController : Controller
    {
        private readonly IUserService _giraf;
        private readonly IGirafUserRepository _userRepository;


        /// <summary>
        /// Constructor for StatusController
        /// </summary>
        /// <param name="giraf">Service Injection</param>
        public StatusController(IUserService giraf, IGirafUserRepository userRepository)
        {
            _giraf = giraf;
            _userRepository = userRepository;
        }

        /// <summary>
        /// End-point for checking if the API is running
        /// </summary>
        /// <returns>Success Reponse.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        public ActionResult Status()
        {
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
                _userRepository.GetAll().FirstOrDefault();
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
                string gitpath;
                string commitHash;

                try
                {
                    gitpath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                    // Check if it is in a build and not server
                    if (gitpath.Contains("bin"))
                    {
                        gitpath = Path.GetFullPath(Path.Combine(gitpath, @"..\..\..\"));
                    }

                    // Get the hidden .git folder
                    gitpath += ".git/";
                    // Get commit hash from the HEAD file in the .git folder.
                    commitHash = System.IO.File.ReadLines(gitpath + "HEAD").First();
                }
                //This occurs when function is run by GitHub Actions
                catch (DirectoryNotFoundException)
                {
                    return StatusCode(StatusCodes.Status204NoContent,$"Could not find directory." );
                }
                // Could potentially add more exceptions to the catch to not return internal server error and be more descriptive
                
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
