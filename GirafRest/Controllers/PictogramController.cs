using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class PictogramController : Controller
    {
        private readonly GirafController _giraf;

        public PictogramController(GirafDbContext context, UserManager<GirafUser> userManager,
            ILoggerFactory loggerFactory) 
        {
            _giraf = new GirafController(context, userManager, loggerFactory.CreateLogger<PictogramController>());
        }

        /// <summary>
        /// Get all public <see cref="Pictogram"/> pictograms available to the user
        /// (i.e the public pictograms and those owned by the user and his department).
        /// </summary>
        /// <returns> All the user's <see cref="Pictogram"/> pictograms.</returns>
        [HttpGet]
        public async Task<IActionResult> ReadPictograms()
        {
            var userPictograms = await ReadAllPictograms();

            var titleQuery = HttpContext.Request.Query["title"];
            if(!String.IsNullOrEmpty(titleQuery)) userPictograms = FilterByTitle(userPictograms, titleQuery);

            return Ok(userPictograms);
        }

        /// <summary>
        /// Read the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and
        /// check if the user is authorized to see it.
        /// </summary>
        /// <param name="id">The ID of the pictogram to fetch.</param>
        /// <returns> The <see cref="Pictogram"/> pictogram with the specified ID,
        /// NotFound (404) if no such <see cref="Pictogram"/> pictogram exists,
        /// BadRequest if the <see cref="Pictogram"/> was not succesfully uploaded to the server or
        /// Unauthorized if the pictogram is private and user does not own it.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> ReadPictogram(int id)
        {
            //Fetch the pictogram and check that it actually exists
            var _pictogram = await _giraf._context.Pictograms
                .Where(p => p.Id == id)
                .FirstAsync();
            if(_pictogram == null) return NotFound();

            //Check if the pictogram is public and return it if so
            if(_pictogram.AccessLevel == AccessLevel.PUBLIC) return Ok(new PictogramDTO(_pictogram, _pictogram.Image));

            var ownsResource = await _giraf.CheckForResourceOwnership(_pictogram, HttpContext);
            if(ownsResource) return Ok(new PictogramDTO(_pictogram, _pictogram.Image));
            else return Unauthorized();
        }

        /// <summary>
        /// Create a new <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all relevant information about the new pictogram.</param>
        /// <returns>The new pictogram with all database-generated information.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePictogram(PictogramDTO pictogram)
        {
            if(pictogram == null) return BadRequest("The body of the request must contain a pictogram.");
            //Create the actual pictogram instance
            Pictogram pict = new Pictogram(pictogram.Title, pictogram.AccessLevel);

            var user = await _giraf.LoadUserAsync(HttpContext.User);

            if(user != null) {
                //Add the pictogram to the current user and his department
                new UserResource(user, pict);
                new DepartmentResource(user.Department, pict);
            }
            else if(pictogram.AccessLevel != AccessLevel.PUBLIC) {
                return BadRequest("You must be logged in to create non-public pictograms.");
            }

            //Stamp the pictogram with current time and add it to the database
            pict.LastEdit = DateTime.Now;
            var res = await _giraf._context.Pictograms.AddAsync(pict);
            await _giraf._context.SaveChangesAsync();

            return Ok(new PictogramDTO(res.Entity));
        }

        /// <summary>
        /// Update info of a <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all new information to update with.
        /// The Id found in this DTO is the target pictogram.
        /// </param>
        /// <returns>NotFound if there is no pictogram with the specified id or 
        /// the updated pictogram to maintain statelessness.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePictogramInfo([FromBody] PictogramDTO pictogram)
        {
            //Fetch the pictogram from the database and check that it exists
            var pict = await _giraf._context.Pictograms.Where(pic => pic.Id == pictogram.Id).FirstAsync();
            if(pict == null) return NotFound();

            //Update the existing database entry and save the changes.
            pict.Merge(pictogram);
            var res = _giraf._context.Pictograms.Update(pict);
            await _giraf._context.SaveChangesAsync();

            return Ok(new PictogramDTO(pict));
        }

        /// <summary>
        /// Delete the <see cref="Pictogram"/> pictogram with the specified id.
        /// </summary>
        /// <param name="id">The id of the pictogram to delete.</param>
        /// <returns>Ok if the pictogram was deleted and NotFound if no pictogram with the id exists.</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePictogram(int id)
        {
            //Fetch the pictogram from the database and check that it exists
            var pict = await _giraf._context.Pictograms.Where(pic => pic.Id == id).FirstAsync();
            if(pict == null) return NotFound();

            if(! await _giraf.CheckForResourceOwnership(pict, HttpContext)) return Unauthorized();

            //Remove it and save changes
            _giraf._context.Pictograms.Remove(pict);
            await _giraf._context.SaveChangesAsync();
            return Ok();
        }

        #region ImageHandling
        /// <summary>
        /// Upload an image for the <see cref="Pictogram"/> pictogram with the given id.
        /// </summary>
        /// <param name="id">Id of the pictogram to upload an image for.</param>
        /// <returns>The pictogram's information along with its image.</returns>
        [HttpPost("image/{id}")]
        [Consumes("image/png")]
        [Authorize]
        public async Task<IActionResult> CreateImage(long id)
        {
            //Fetch the image and check that it exists
            var pict = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstAsync();
            if(pict == null) return NotFound();
            else if(pict.Image != null) return BadRequest("The pictogram already has an image.");

            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            pict.Image = image;
            var pictoResult = await _giraf._context.SaveChangesAsync();

            return Ok(new PictogramDTO(pict, image));
        }

        /// <summary>
        /// Update the image of a <see cref="Pictogram"/> pictogram with the given Id.
        /// </summary>
        /// <param name="id">Id of the pictogram to update the image for.</param>
        /// <returns>The updated pictogram along with its image.</returns>
        [Consumes("image/png")]
        [HttpPut("image/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePictogramImage(long id) {
            var picto = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstAsync();
            if(picto == null) return NotFound();
            else if(picto.Image == null) return BadRequest("The pictogram does not have a image, please POST instead.");

            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            picto.Image = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(new PictogramDTO(picto, image));
        }

        [HttpGet("image/{id}")]
        public async Task<FileResult> ReadPictogramImage(long id) {
            var picto = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstAsync();
            
            return base.File(picto.Image, "image/png");
        }
        #endregion

        #region helpers

        private async Task<List<PictogramDTO>> ReadAllPictograms() {
            //Fetch all public pictograms and cask to a list - using Union'ing two IEnumerables gives an exception.
            var _pictograms = await _giraf._context.Pictograms
                .Where(p => p.AccessLevel == AccessLevel.PUBLIC)
                .ToListAsync();
            
            //Find the user and add his pictograms to the result
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if(user != null)
            {
                _giraf._logger.LogInformation($"Fetching user pictograms for user {user.UserName}");
                var userPictograms = user.Resources
                    .Select(ur => ur.Resource)
                    .OfType<Pictogram>();
                _pictograms = _pictograms
                    .Union(userPictograms)
                    .ToList();
                //Also find his department and their pictograms
                var dep = user.Department;
                if(dep != null){
                    _giraf._logger.LogInformation($"Fetching pictograms for department {dep.Name}");
                    var depPictograms = dep.Resources
                        .Select(dr => dr.Resource)
                        .OfType<Pictogram>();
                _pictograms = _pictograms
                    .Union (depPictograms)
                    .ToList();
                }
                else _giraf._logger.LogWarning($"{user.UserName} has no department.");
            }
            
            //Return the list of pictograms as Pictogram DTOs
            //- returning Pictograms directly causes an exception due to circular references
            return _pictograms.Select(p => new PictogramDTO(p)).ToList();
        }
        #endregion
        #region query filters
        public List<PictogramDTO> FilterByTitle(List<PictogramDTO> pictos, string titleQuery) { 
            var matches = pictos
                .Where(p => p.Title.ToLower().Contains(titleQuery.ToLower()));

            return matches.ToList();
        }
        #endregion
    }
}