using GirafRest.Setup;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class SequenceController : GirafController
    {
        public SequenceController(GirafDbContext context, UserManager<GirafUser> userManager,
            IHostingEnvironment env, ILoggerFactory loggerFactory)
                : base(context, userManager, env, loggerFactory.CreateLogger<SequenceController>())
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Sequences.Where(s => s.AccessLevel == AccessLevel.PUBLIC).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSequence([FromBody] SequenceDTO DTO)
        {
            var _pictogram = await _context.Pictograms.Where(p => p.Key == DTO.ThumbnailID).FirstAsync();
            Sequence _sequence = new Sequence(DTO.Title, DTO.AccessLevel, _pictogram);
            _sequence.LastEdit = DateTime.Now;
            var res = await _context.Sequences.AddAsync(_sequence);

            _context.SaveChanges();
            return Ok(res.Entity);
        }
    }
}