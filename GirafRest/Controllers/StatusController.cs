using System.Net;
using System.Linq;
using GirafRest.Services;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        /// Reference to the GirafService, which contains helper methods used by most controllers.
        /// </summary>
        private readonly IGirafService _giraf;
        /// <summary>
        /// 
        public StatusController(IGirafService giraf)
        {
            _giraf = giraf;
        }
        [HttpGet("")]
        public HttpStatusCode Status()
        {
            return HttpStatusCode.OK;
        }

        [HttpGet("database")]
        public HttpStatusCode DatabaseStatus()
        {
            try
            {
                _giraf._context.Users.FirstOrDefault();
                return HttpStatusCode.OK;
            }
            catch (System.Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
