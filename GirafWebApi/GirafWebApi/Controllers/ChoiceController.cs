using System.Linq;
using GirafWebApi.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace GirafWebApi.Controllers
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
            return Ok(_context.Choices.Where(ch => ch.Key == ID));  
        }
    }
}