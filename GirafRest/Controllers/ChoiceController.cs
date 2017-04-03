using System.Linq;
using GirafRest.Data;
using GirafRest.Setup;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class ChoiceController : Controller
    {
        GirafDbContext _context;
        public ChoiceController(GirafDbContext context)
        {
            _context = context;
        }  
        [HttpGet]
        public IActionResult Get(long ID)
        {
            return Ok(_context.Choices.Where(ch => ch.Id == ID));  
        }
    }
}