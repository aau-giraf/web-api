using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Controllers
{
    [Route("v1/[controller]")]
    public class StatusController : Controller
    {
        private readonly IGirafService _giraf;
        public StatusController(IGirafService giraf)
        {
            _giraf = giraf;
        }

        /// <summary>
        /// End-point for checking if the API is running
        /// </summary>
        /// <returns>Success Reponse.</returns>
        [HttpGet("")]
        public ActionResult Status()
        {
            return Ok(new MyResponse("GIRAF API is running!"));
        }

        /// <summary>
        /// End-point for checking connection to the database
        /// </summary>
        /// <returns>Success response if connection to database else ErrorResponse</returns>
        [HttpGet("database")]
        public ActionResult DatabaseStatus()
        {
            try
            {
                _giraf._context.Users.FirstOrDefault();
                return Ok(new MyResponse("Connection to database"));
            }
            catch (System.Exception e)
            {
                return StatusCode(503, new RESTError(ErrorCode.Error, "Error when connecting to database", e.Message));
            }
        }

        /// <summary>
        /// Endpoint for getting git version info i.e. branch and commithash 
        /// </summary>
        /// <returns>branch and commit hash for this API instance</returns>
        [HttpGet("version-info")]
        public ActionResult GetVersionInfo()
        {
            var gitpath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/.git/";
            var pathToHead = System.IO.File.ReadLines(gitpath + "HEAD").First().Split(" ").Last();

            var hash = System.IO.File.ReadLines(gitpath + pathToHead).First();
            // this assumes that branches are not named with / however this should be enforced anyways
            var branch = pathToHead.Split("/").Last();
            return Ok(new MyResponse($"Branch: {branch} CommitHash: {hash}"));
        }
    }
}
