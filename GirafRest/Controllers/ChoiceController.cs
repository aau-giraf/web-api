using System.Linq;
using GirafRest.Data;
using GirafRest.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GirafRest.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using GirafRest.Models.DTOs;
using System;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class ChoiceController : GirafController
    {
        public ChoiceController(GirafDbContext context, UserManager<GirafUser> userManager,
            IHostingEnvironment env, ILoggerFactory loggerFactory)
                : base(context, userManager, env, loggerFactory.CreateLogger<ChoiceController>())
        {
        }

        /// <summary>
        /// Read the <see cref="Choice"/> choice with the specified <paramref name="ID"/> ID and
        /// check if the user is authorized to see it.
        /// </summary>
        /// <param name="ID"> The ID of the choice to fetch.</param>
        /// <returns> The <see cref="Choice"/> choice with the specified ID,
        /// NotFound (404) if no such <see cref="Choice"/> choice exists or
        /// Unauthorized if the <see cref="Choice"/> choice is private and user does not own it.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> ReadChoice(long id)
        {
            _logger.LogInformation($"Fetching choice which match the ID");
            Choice _choice = await _context.Choices.Where(ch => ch.Id == id).Include(ch => ch.Options).FirstAsync();
            if (_choice == null) NotFound();
            
            _logger.LogInformation($"Cheching if current user have access to all choices");
            foreach (PictoFrame p in _choice)
            {
                if (p.AccessLevel != AccessLevel.PUBLIC && !(await CheckForResourceOwnership(p)))
                    return Unauthorized();
            }

            return Ok(_choice);  
        }
        
        /// <summary>
        /// Create a new <see cref="Choice"/> choice.
        /// </summary>
        /// <param name="choice"> A <see cref="ChoiceDTO"/> with all relevant information about the new choíce.</param>
        /// <returns>The new choice with all database-generated information.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateChoice([FromBody] ChoiceDTO choice)
        {
            Choice _choice = new Choice(choice.Options);

            // need to relate the choice to him and his department?
            
            _choice.LastEdit = DateTime.Now;
            _logger.LogInformation($"Adding the new choice to the database");
            var res = await _context.Choices.AddAsync(_choice);
            await _context.SaveChangesAsync();

            return Ok(new ChoiceDTO(res.Entity));
        }

        /// <summary>
        /// Update info of a <see cref="Choice"/> choice.
        /// </summary>
        /// <param name="choice"> A <see cref="ChoiceDTO"/> with all new information to update with.
        /// The Id found in this DTO is the target choice.
        /// </param>
        /// <returns> NotFound, if there is no choice with the specified id, or 
        /// the updated choice, to maintain statelessness.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateChoiceInfo([FromBody] ChoiceDTO choice)
        {
            Choice _choice = await _context.Choices.Where(ch => ch.Id == choice.Id).FirstAsync();
            if (_choice == null) NotFound();

            _logger.LogInformation($"Updating the choice with the new information and adding it to the database");
            _choice.Merge(choice);
            _context.Choices.Update(_choice);
            _context.SaveChanges();

            return Ok(new ChoiceDTO(_choice));
        }

        /// <summary>
        /// Delete the <see cref="Choice"/> choice with the specified id.
        /// </summary>
        /// <param name="id"> The id of the choice to delete.</param>
        /// <returns> Ok if the choice was deleted after checking authorization and NotFound if no choice with the id exists.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChoice(long id)
        {
            var _choice = await _context.Choices.Where(ch => ch.Id == id).FirstAsync();
            if (_choice == null) NotFound();

            _logger.LogInformation($"Checking if the user is authorized");
            foreach (PictoFrame p in _choice)
            {
                if (!(await CheckForResourceOwnership(p))) Unauthorized();
            }

            _logger.LogInformation($"Removing selected choice from the database");
            _context.Choices.Remove(_choice);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}