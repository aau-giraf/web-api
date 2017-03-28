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
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUserSequences()
        {
            var user = await LoadUserAsync(HttpContext.User);
            if(user == null)
                return NoContent();
            var userSequences = user.Resources
                    .Select(ur => ur.Resource)
                    .OfType<Sequence>();
            var sequences = await _context.Sequences
                .Where(s => s.AccessLevel == AccessLevel.PUBLIC)
                .ToListAsync();
            sequences = sequences.Union(userSequences).ToList();
            return Ok(userSequences.Select(s => new SequenceDTO(s)).ToList());
        }

        [HttpGet("department")]
        [Authorize]
        public async Task<IActionResult> GetDepartmentSequences()
        {
            var user = await LoadUserAsync(HttpContext.User);
            if (user == null)
                return NoContent();
            var departSequences = await _context.DeparmentResources
                    .Select(dep => dep.Resource)
                    .OfType<Sequence>()
                    .ToListAsync();
            var sequences = await GetPublicSequences();
            sequences = sequences.Union(departSequences).ToList();
            return Ok(sequences.Select(s => new SequenceDTO(s)).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSequence([FromBody] SequenceDTO DTO)
        {
            var _pictogram = await _context.Pictograms.Where(p => p.Key == DTO.ThumbnailID).FirstAsync();
            Sequence _sequence = new Sequence(DTO.Title, DTO.AccessLevel, _pictogram, DTO.Elements);
            _sequence.LastEdit = DateTime.Now;
            var res = await _context.Sequences.AddAsync(_sequence);

            _context.SaveChanges();
            return Ok(res.Entity);
        }

        
        [HttpPut]
        public async Task<IActionResult> UpdateSequence([FromBody] SequenceDTO DTO)
        {
            if (DTO == null)
                return BadRequest();
            var _sequence = await _context.Sequences.Where(s => s.Key == DTO.Id).FirstAsync();
            if (_sequence == null)
                return BadRequest();
            _sequence.Merge(DTO);
            return Ok();
        }

        private async Task<List<Sequence>> GetPublicSequences()
        {
            return(await _context.Sequences
                .Where(s => s.AccessLevel == AccessLevel.PUBLIC)
                .ToListAsync());
        }
    }
}