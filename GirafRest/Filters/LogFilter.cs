using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GirafRest.Filters
{
    /// <summary>
    /// Implementation of IActionFilter, for filtering based on Claim for current User
    /// </summary>
    public class LogFilter : IActionFilter
    {
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
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null) {
                throw new System.ArgumentNullException(context + " is null");
            }
            string path = "Logs/log-" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".txt";
            var controller = context.Controller as Controller;
            string userId = controller.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == userId)?.UserName;
            string p = context.HttpContext.Request.Path;
            string verb = context.HttpContext.Request.Method;
            var action = context.ActionDescriptor.DisplayName;
            var error = ((context.Result as ObjectResult)?.Value as ErrorResponse)?.ErrorCode.ToString();
            string[] lines = new string[]
            {
                $"{DateTime.UtcNow:o}; {user}; {userId}; {verb}; {p}; {error}"
            };
            lock (filelock)
            {
                Directory.CreateDirectory("Logs");
                File.AppendAllLines(path, lines);
            }
        }
    }

}
