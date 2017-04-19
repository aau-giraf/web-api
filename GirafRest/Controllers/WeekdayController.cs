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
using GirafRest.Services;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class WeekdayController : Controller
    {
        private readonly IGirafService _giraf;
        public WeekdayController(IGirafService giraf, ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Weekday");
        }

        /*[HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Weekdays.Where(s => s.AccessLevel == AccessLevel.PUBLIC).ToListAsync());
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUserWeekdays()
        {
            var user = await LoadUserAsync(HttpContext.User);
            if(user == null)
                return NoContent();
            var userSequences = user.Resources
                    .Select(ur => ur.Resource)
                    .OfType<Weekday>();
            var sequences = await _context.Weekdays
                .Where(s => s.AccessLevel == AccessLevel.PUBLIC)
                .ToListAsync();
            sequences = sequences.Union(userSequences).ToList();
            return Ok(userSequences.Select(s => new WeekdayDTO(s)).ToList());
        }

        [HttpGet("department")]
        [Authorize]
        public async Task<IActionResult> GetDepartmentWeekdays()
        {
            var user = await LoadUserAsync(HttpContext.User);
            if (user == null)
                return NoContent();
            var departSequences = await _context.DeparmentResources
                    .Select(dep => dep.Resource)
                    .OfType<Weekday>()
                    .ToListAsync();
            var sequences = await GetPublicSequences();
            sequences = sequences.Union(departSequences).ToList();
            return Ok(sequences.Select(s => new WeekdayDTO(s)).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CreateWeekday([FromBody] WeekdayDTO DTO)
        {
            var _pictogram = await _context.Pictograms.Where(p => p.Key == DTO.ThumbnailID).FirstAsync();
            Weekday _weekday = new Weekday(DTO.Title, DTO.AccessLevel, _pictogram, DTO._elements);
            _weekday.LastEdit = DateTime.Now;
            var res = await _context.Weekdays.AddAsync(_weekday);

            _context.SaveChanges();
            return Ok(res.Entity);
        }*/

        
        [HttpPut]
        public async Task<IActionResult> UpdateWeekday([FromBody] WeekdayDTO DTO)
        {
            if (DTO == null)
                return BadRequest();
            var _week = await _giraf._context.Weeks.FirstAsync();
            if (_week == null)
                return BadRequest();
            _week.Weekdays.Where(d => d.Day == DTO.Day).First();
            return Ok(_week);
        }

       /* private async Task<List<Weekday>> GetPublicSequences()
        {
            return(await _context.Weekdays
                .Where(s => s.AccessLevel == AccessLevel.PUBLIC)
                .ToListAsync());
        }*/
    }
}