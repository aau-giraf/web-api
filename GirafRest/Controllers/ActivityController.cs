using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Authorization;

namespace GirafRest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : Controller
    {
        private readonly GirafDbContext _context;

        public ActivityController(GirafDbContext context)
        {
            _context = context;
        }

        // POST: api/Activity
        [HttpPost("{userId}/week/{weekYear}/{weekNumber}/{weekDay}")]
        public async Task<ActionResult<Activity>> PostActivity([FromBody] Activity activity, string userId, int weekYear, int weekNumber, Days weekday)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActivity", new { id = activity.Key }, activity);
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Activity>> DeleteActivity(long id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return activity;
        }

        private bool ActivityExists(long id)
        {
            return _context.Activities.Any(e => e.Key == id);
        }
    }
}
