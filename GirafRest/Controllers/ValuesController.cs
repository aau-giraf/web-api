using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ValuesController : Controller
    {
        private string[] values = { "Hest", "Mel", "Lotte" };

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(values);
        }
    }
}
