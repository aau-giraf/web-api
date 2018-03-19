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
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The ChoiceController serves the purpose of presenting choices on request. It also allows the user to
    /// select either of the options in a choice.
    /// </summary>
    [Route("v1/[controller]")]
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
        /// Read the <see cref="Choice"/> choice with the specified <paramref name="id"/> ID and
        /// check if the user is authorized to see it.
        /// </summary>
        /// <param name="ID"> The ID of the choice to fetch.</param>
        /// <returns> 
        /// Response with the ChoiceDTO requested
        /// NotFound if no choice was found
        /// NotAuthorized the user does no have access to the found choice
        /// </returns>
        [HttpGet("{id}")]
        public async Task<Response<ChoiceDTO>> ReadChoice(long id)
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
                return new ErrorResponse<ChoiceDTO>(ErrorCode.NotFound);
            }
            if (!(await checkAccess(_choice))) return new ErrorResponse<ChoiceDTO>(ErrorCode.NotAuthorized);

            return new Response<ChoiceDTO>(new ChoiceDTO(_choice));  
        }
        
        /// <summary>
        /// Create a new <see cref="Choice"/> choice.
        /// </summary>
        /// <param name="choice"> A <see cref="ChoiceDTO"/> with all relevant information about the new choice.</param>
        /// <returns> 
        /// Response with ChoiceDTO containing the created choice if succeeded
        /// FormatError if the argument was null
        /// NotAuthorized if the user does not have access to some of the choices
        /// ChoiceContainsInvalidPictogramId if the supplied ID was not found
        /// </returns>
        [HttpPost("")]
        public async Task<Response<ChoiceDTO>> CreateChoice([FromBody] ChoiceDTO choice)
        {
            //Check if a valid ChoiceDTO has been specified
            if (choice == null)
                return new ErrorResponse<ChoiceDTO>(ErrorCode.FormatError);
            
            //Find all the involved resources and check that they exist
            List<Pictogram> pictogramList = new List<Pictogram>();
            if(choice.Options != null)
            {
                foreach (var option in choice.Options) 
                {
                    var pf = await _giraf._context.Pictograms
                        .Where(p => p.Id == option.Id)
                        .FirstOrDefaultAsync();
                    if (pf == null)
                        return new ErrorResponse<ChoiceDTO>(ErrorCode.ChoiceContainsInvalidPictogramId, $"{option.Id}");
                    pictogramList.Add(pf);
                } 
            }

            //Create an object for the choice
            Choice _choice = new Choice(pictogramList, choice.Title);
            //Check if the user has access to all options of the choice
            if (!(await checkAccess(_choice))) return new ErrorResponse<ChoiceDTO>(ErrorCode.NotAuthorized);

            //Add the choice to the database
            _giraf._logger.LogInformation("Adding the new choice to the database");
            var res = await _giraf._context.Choices.AddAsync(_choice);
            await _giraf._context.SaveChangesAsync();

            return new Response<ChoiceDTO>(new ChoiceDTO(_choice));
        }

        /// <summary>
        /// Update info of a <see cref="Choice"/> choice.
        /// </summary>
        /// <param name="newValues"> A <see cref="ChoiceDTO"/> with all new information to update with.
        /// The Id found in this DTO is the target choice.
        /// </param>
        /// <returns>
        /// Response containing the updated choice
        /// FormatError if the argument was null
        /// NotAuthorized if the user does not own the choice
        /// ChoiceContainsInvalidPictogramId if the pictogram could not be found
        /// </returns>
        [HttpPut("{id}")]
        public async Task<Response<ChoiceDTO>> UpdateChoice(long id, [FromBody] ChoiceDTO newValues)
        {
            //Check if a valid ChoiceDTO has been specified
            if (newValues == null)
                return new ErrorResponse<ChoiceDTO>(ErrorCode.FormatError);

            //Attempt to find the target choice.
            Choice choice = await _giraf._context.Choices
                .Where(ch => ch.Id == id)
                .Include(ch => ch.Options)
                .ThenInclude(op => op.Resource)
                .FirstOrDefaultAsync();
            
            if (choice == null)
                return new ErrorResponse<ChoiceDTO>(ErrorCode.NotFound, $"ID={id} not found");
            
            //Check that the user actually owns the choice
            if (!(await checkAccess(choice))) 
                return new ErrorResponse<ChoiceDTO>(ErrorCode.NotAuthorized);

            //Find all the involved resources and check that they exist
            List<Pictogram> pictogramList = new List<Pictogram>();
            if(newValues.Options != null)
            {
                foreach (var option in newValues.Options)
                {
                    var pf = await _giraf._context.Pictograms
                        .Where(p => p.Id == option.Id)
                        .FirstOrDefaultAsync();
                    if (pf == null)
                        return new ErrorResponse<ChoiceDTO>(ErrorCode.ChoiceContainsInvalidPictogramId, $"{option.Id}");
                    pictogramList.Add(pf);
                }
            }
            
            //Check that the user has access to all pictograms that were added to the choice
            if (!(await checkAccess(choice))) 
                return new ErrorResponse<ChoiceDTO>(ErrorCode.NotAuthorized);
            
            //Modify the choice 
            choice.Clear();
            choice.AddAll(pictogramList);

            //Save the changes
            _giraf._logger.LogInformation($"Updating the choice with the new information and adding it to the database");
            choice.Merge(newValues);
            _giraf._context.Choices.Update(choice);
            _giraf._context.SaveChanges();

            return new Response<ChoiceDTO>(new ChoiceDTO(choice));
        }

        /// <summary>
        /// Delete the <see cref="Choice"/> choice with the specified id.
        /// </summary>
        /// <param name="id">The id of the choice to delete.</param>
        /// <returns>
        /// Empty Response on success
        /// NotFound if the choice was not found
        /// NotAuthorized if the user does not own the choice
        /// </returns>
        [Authorize(Policy = GirafRole.RequireGuardianOrSuperUser)]
        [HttpDelete("{id}")]
        public async Task<Response> DeleteChoice(long id)
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
                return new ErrorResponse<ChoiceDTO>(ErrorCode.NotFound, $"ID={id} not found");
            }

            //Check if the user is authorized to delete it
            if(!(await checkAccess(_choice))) return new ErrorResponse<ChoiceDTO>(ErrorCode.NotAuthorized);

            //Remove it from the database
            _giraf._logger.LogInformation($"Removing selected choice from the database");
            _giraf._context.Choices.Remove(_choice);
            _giraf._context.SaveChangesAsync();

            return new Response();
        }

        /// <summary>
        /// Check if the current user has access to all resources in Choice.Option.
        /// </summary>
        /// <param name="choice">A reference to the choice to check ownership for.</param>
        /// <returns>
        /// True if the user owns all the involved resources, false if not.
        /// </returns>
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