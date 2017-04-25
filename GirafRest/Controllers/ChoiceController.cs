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

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class ChoiceController : Controller
    {
        private readonly IGirafService _giraf;

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
                _choice = await _giraf._context.Choices.Where(ch => ch.Id == id).Include(ch => ch.Options).ThenInclude(op => op.Resource).FirstAsync();
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
        /// <param name="choice"> A <see cref="ChoiceDTO"/> with all relevant information about the new choíce.</param>
        /// <returns>The new choice with all database-generated information.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateChoice([FromBody] ChoiceDTO choice)
        {
            List<PictoFrame> pictoFrameList = new List<PictoFrame>();
            try
            {
                foreach (var option in choice.Options)
                    pictoFrameList.Add(await _giraf._context.PictoFrames.Where(p => p.Id == option.Id).FirstAsync());
            }
            catch (Exception)
            {
                return NotFound();
            }
            Choice _choice = new Choice(pictoFrameList);
            if (!(await checkAccess(_choice))) return Unauthorized();
            _choice.LastEdit = DateTime.Now;
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
        /// <returns> NotFound, if there is no choice with the specified id, or 
        /// the updated choice, to maintain statelessness.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateChoiceInfo([FromBody] ChoiceDTO choice)
        {
            Choice _choice;
            List<PictoFrame> pictoFrameList = new List<PictoFrame>();
            try
            {
                _choice = await _giraf._context.Choices.Where(ch => ch.Id == choice.Id).Include(ch => ch.Options).ThenInclude(op => op.Resource).FirstAsync();
                foreach (var option in choice.Options)
                {
                    pictoFrameList.Add(await _giraf._context.PictoFrames.Where(p => p.Id == option.Id).FirstAsync());
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
            if (!(await checkAccess(_choice))) return Unauthorized();
            _choice.Clear();
            _choice.AddAll(pictoFrameList);
            if (!(await checkAccess(_choice))) return Unauthorized();
            _giraf._logger.LogInformation($"Updating the choice with the new information and adding it to the database");
            _choice.Merge(choice);
            _giraf._context.Choices.Update(_choice);
            _giraf._context.SaveChanges();

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
            Choice _choice;
            try
            {
                _choice = await _giraf._context.Choices.Where(ch => ch.Id == id).Include(ch => ch.Options).ThenInclude(op => op.Resource).FirstAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }
            if(!(await checkAccess(_choice))) return Unauthorized();
            _giraf._logger.LogInformation($"Removing selected choice from the database");
            _giraf._context.Choices.Remove(_choice);
            await _giraf._context.SaveChangesAsync();

            return Ok();
        }

        public async Task<bool> checkAccess(Choice choice)
        {
            _giraf._logger.LogInformation($"Checking if the user is authorized");
            foreach (PictoFrame p in choice)
            {
                bool ownsResource;
                switch (p.AccessLevel)
                {
                    case AccessLevel.PROTECTED:
                        ownsResource = await _giraf.CheckProtectedOwnership(p, HttpContext);
                        break;
                    case AccessLevel.PRIVATE:
                        ownsResource = await _giraf.CheckPrivateOwnership(p, HttpContext);
                        if (!ownsResource)
                            return false;
                        break;
                    case AccessLevel.PUBLIC:
                    default:
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