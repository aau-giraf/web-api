using GirafRest.Models.Responses;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GirafRest.IRepositories;

namespace GirafRest.Filters
{
    /// <summary>
    /// Implementation of IActionFilter, for filtering based on Claim for current User
    /// </summary>
    public class LogFilter : IActionFilter
    {
        private readonly IGirafUserRepository _girafUserRepository;
        
        IGirafService _giraf;
        
        /// <summary>
        /// Initialize for LogFilter
        /// </summary>
        /// <param name="giraf">Giraf Service</param>
        public LogFilter(IGirafService giraf)
        {
            _giraf = giraf;
        }

        /// <summary>
        /// On executing hook
        /// </summary>
        /// <param name="context">Context in which is executed</param>
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        private static readonly object filelock = new object();

        /// <summary>
        /// Post-execution hook, writing to log file
        /// </summary>
        /// <param name="context">Context in which is executed</param>
        public async void OnActionExecuted(ActionExecutedContext context)
        {
            string path = "Logs/log-" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".txt";
            var controller = context.Controller as Controller;
            string userId = controller.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await _girafUserRepository.GetUserWithId(userId);
            var userName = user?.UserName;
            string p = context.HttpContext.Request.Path;
            string verb = context.HttpContext.Request.Method;
            var action = context.ActionDescriptor.DisplayName;
            var error = ((context.Result as ObjectResult)?.Value as ErrorResponse)?.ErrorCode.ToString();
            string[] lines = new string[]
            {
                $"{DateTime.UtcNow:o}; {userName}; {userId}; {verb}; {p}; {error}"
            };
            lock (filelock)
            {
                Directory.CreateDirectory("Logs");
                File.AppendAllLines(path, lines);
            }
        }
    }

}
