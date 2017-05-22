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
using GirafRest.Models.Many_to_Many_Relationships;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The ChoiceController serves the purpose of presenting choices on request. It also allows the user to
    /// select either of the options in a choice.
    /// </summary>
    [Route("[controller]")]
    public class ChoiceController : Controller
    {
        /// <summary>
        /// A reference to the Giraf service that implements common functionality for all controllers.
        /// </summary>
        private readonly IGirafService _giraf;

        /// <summary>
        /// Creates a new ChoiceController. The choice controller allows the users to see their choices and
        /// also confirm or reject choices. As with all other controllers, this one is also instantiated by ASP.NET.
        /// </summary>
        /// <param name="girafService">A reference to the GirafService.</param>
        /// <param name="lFactory">A reference to a logger factory, that enables uniform logging from all controllers.</param>
        public ChoiceController(IGirafService girafService, ILoggerFactory lFactory)
        {
            _giraf = girafService;
            _giraf._logger = lFactory.CreateLogger("Choice");
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
            _giraf._logger.LogInformation($"Fetching choice which match the ID");
            Choice _choice;
            try
            {
                _choice = await _giraf._context.Choices
                    .Where(ch => ch.Id == id)
                    .Include(ch => ch.Options)
                    .ThenInclude(op => op.Resource)
                    .FirstAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }
            if (!(await checkAccess(_choice))) return Unauthorized();

            return Ok(new ChoiceDTO(_choice));  
        }
        
        /// <summary>
        /// Create a new <see cref="Choice"/> choice.
        /// </summary>
        /// <param name="choice"> A <see cref="ChoiceDTO"/> with all relevant information about the new cho�ce.</param>
        /// <returns>
        /// BadRequest if no valid ChoiceDTO is supplied, NotFound if the list of options contains an invalid pictogram
        /// id and Ok with the new choice with all database-generated information if the creation succeeded.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateChoice([FromBody] ChoiceDTO choice)
        {
            //Check if a valid ChoiceDTO has been specified
            if (choice == null)
                return BadRequest("Could not find a valid ChoiceDTO in the body of the request.");

            //Attempt to find all resources that make up the choice
            List<Pictogram> pictogramList = new List<Pictogram>();
            try
            {
                foreach (var option in choice.Options)
                    pictogramList.Add(await _giraf._context.Pictograms.Where(p => p.Id == option.Id).FirstAsync());
            }
            catch
            {
                return NotFound("The choice options contains an id of a pictogram that does not exist.");
            }

            //Create an object for the choice
            Choice _choice = new Choice(pictogramList, choice.Title);
            //Check if the user has access to all options of the choice
            if (!(await checkAccess(_choice))) return Unauthorized();

            //Add the choice to the database
            _giraf._logger.LogInformation($"Adding the new choice to the database");
            var res = await _giraf._context.Choices.AddAsync(_choice);
            await _giraf._context.SaveChangesAsync();

            return Ok(new ChoiceDTO(_choice));
        }

        /// <summary>
        /// Update info of a <see cref="Choice"/> choice.
        /// </summary>
        /// <param name="choice"> A <see cref="ChoiceDTO"/> with all new information to update with.
        /// The Id found in this DTO is the target choice.
        /// </param>
        /// <returns>
        /// NotFound, if there is no choice with the specified id or the list of options contains an invalid pictogram id,
        /// or Ok and the updated choice.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChoice(long id, [FromBody] ChoiceDTO choice)
        {
            //Attempt to find the target choice.
            Choice _choice;
            List<Pictogram> pictogramList = new List<Pictogram>();
            _choice = await _giraf._context.Choices
                .Where(ch => ch.Id == id)
                .Include(ch => ch.Options)
                .ThenInclude(op => op.Resource)
                .FirstOrDefaultAsync();
            if (_choice == null)
                return NotFound("A choice with the given id was not found.");
            //Check that the user actually owns the choice
            if (!(await checkAccess(_choice))) return Unauthorized();

            //Find all the involved resource and check that they exist
            foreach (var option in choice.Options)
            {
                var pf = await _giraf._context.Pictograms
                    .Where(p => p.Id == option.Id)
                    .FirstOrDefaultAsync();
                if (pf == null)
                    return NotFound("The choice contained an id of a nonexisting pictogram.");
                pictogramList.Add(pf);
            }
            
            //Modify the choice and check that the user has access to all pictograms that were added to the choice
            _choice.Clear();
            _choice.AddAll(pictogramList);
            if (!(await checkAccess(_choice))) return Unauthorized();

            //Save the changes
            _giraf._logger.LogInformation($"Updating the choice with the new information and adding it to the database");
            _choice.Merge(choice);
            _giraf._context.Choices.Update(_choice);
            _giraf._context.SaveChanges();

            return Ok(new ChoiceDTO(_choice));
        }

        /// <summary>
        /// Delete the <see cref="Choice"/> choice with the specified id.
        /// </summary>
        /// <param name="id">The id of the choice to delete.</param>
        /// <returns> Ok if the choice was deleted after checking authorization and NotFound if no choice with the id exists.</returns>
        [Authorize(Policy = GirafRole.RequireGuardianOrSuperUser)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChoice(long id)
        {
            //Attempt to retrieve the choice
            Choice _choice;
            try
            {
                _choice = await _giraf._context.Choices
                    .Where(ch => ch.Id == id)
                    .Include(ch => ch.Options)
                    .ThenInclude(op => op.Resource)
                    .FirstAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            //Check if the user is authorized to delete it
            if(!(await checkAccess(_choice))) return Unauthorized();

            //Remove it from the database
            _giraf._logger.LogInformation($"Removing selected choice from the database");
            _giraf._context.Choices.Remove(_choice);
            await _giraf._context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Check if the current user has access to all resources in Choice.Option.
        /// </summary>
        /// <param name="choice">A reference to the choice to check ownership for.</param>
        /// <returns>True if the user owns all the involved resources, false if not.</returns>
        private async Task<bool> checkAccess(Choice choice)
        {
            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            _giraf._logger.LogInformation($"Checking if the user is authorized");
            foreach (Pictogram p in choice)
            {
                bool ownsResource = false;
                switch (p.AccessLevel)
                {
                    case AccessLevel.PROTECTED:
                        ownsResource = await _giraf.CheckProtectedOwnership(p, usr);
                        break;
                    case AccessLevel.PRIVATE:
                        ownsResource = await _giraf.CheckPrivateOwnership(p, usr);
                        if (!ownsResource)
                            return false;
                        break;
                    case AccessLevel.PUBLIC:
                        ownsResource = true;
                        break;
                }
                if (!ownsResource)
                    return false;
            }
            return true;
        }
    }
}