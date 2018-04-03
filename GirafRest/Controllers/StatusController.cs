using System.Net;
using System.Linq;
using GirafRest.Services;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest
{
    [Route("v1/[controller]")]
    public class StatusController : Controller
    {
        private readonly IGirafService _giraf;
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
