using System.Linq;
using System.Net;
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
        [HttpGet("")]
        public Response Status()
        {
            return new Response();
        }

        [HttpGet("database")]
        public Response DatabaseStatus()
        {
            try
            {
                _giraf._context.Users.FirstOrDefault();
                return new Response();
            }
            catch (System.Exception ex)
            {
                return new ErrorResponse(ErrorCode.Error);
            }
        }
    }
}
