using GirafWebApi.Contexts;
using GirafWebApi.Models;
using GirafWebApi.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafWebApi.Controllers
{
    [Route("[controller]")]
    public class SequenceController : Controller
    {
        public readonly GirafDbContext _context;
        public readonly UserManager<GirafUser> _userManager;

        public SequenceController(GirafDbContext context, UserManager<GirafUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Sequences.Where(s => s.AccessLevel == AccessLevel.PUBLIC).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSequence([FromBody] SequenceDTO DTO)
        {
            if (DTO == null) return BadRequest();
            Sequence _sequence = new Sequence(DTO.title, DTO.access_level, DTO.department_id, DTO.owner_id, ( await _context.Pictograms.Where(p => p.Key == DTO.thumbnail_id).FirstAsync()));
            var res = await _context.Sequences.AddAsync(_sequence);
            _context.SaveChanges();
            return Ok(res.Entity);
        }
    }
}