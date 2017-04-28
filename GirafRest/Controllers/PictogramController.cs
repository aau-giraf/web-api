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
using GirafRest.Services;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The pictogram controller fetches an delivers pictograms on request. It also has endpoints for fetching
    /// and uploading images to pictograms. Supported image-types are .png and .jpg.
    /// </summary>
    [Route("[controller]")]
    public class PictogramController : Controller
    {
        /// <summary>
        /// A reference to GirafService, that defines common functionality for all classes.
        /// </summary>
        private readonly IGirafService _giraf;

        public PictogramController(IGirafService girafController, ILoggerFactory lFactory) 
        {
            _giraf = girafController;
            _giraf._logger = lFactory.CreateLogger("Pictogram");
        }

        /// <summary>
        /// Get all public <see cref="Pictogram"/> pictograms available to the user
        /// (i.e the public pictograms and those owned by the user (PRIVATE) and his department (PROTECTED)).
        /// </summary>
        /// <returns> All the user's <see cref="Pictogram"/> pictograms.</returns>
        [HttpGet]
        public async Task<IActionResult> ReadPictograms()
        {
            //Produce a list of all pictograms available to the user
            var userPictograms = await ReadAllPictograms();
            if (userPictograms == null)
                return BadRequest("There is most likely no pictograms available on the server.");

            //Filter out all that does not satisfy the query string, if such is present.
            var titleQuery = HttpContext.Request.Query["title"];
            if(!String.IsNullOrEmpty(titleQuery)) userPictograms = FilterByTitle(userPictograms, titleQuery);

            if (userPictograms.Count == 0)
                return NotFound();
            else
                return Ok(userPictograms.Select(p => new PictogramDTO(p, p.Image)).ToList());
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
        public async Task<IActionResult> ReadPictogram(long id)
        {
            try
            {
                //Fetch the pictogram and check that it actually exists
                var _pictogram = await _giraf._context.Pictograms
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                if (_pictogram == null) return NotFound();

                //Check if the pictogram is public and return it if so
                if (_pictogram.AccessLevel == AccessLevel.PUBLIC) return Ok(new PictogramDTO(_pictogram, _pictogram.Image));

                bool ownsResource = false;
                if (_pictogram.AccessLevel == AccessLevel.PRIVATE)
                    ownsResource = await _giraf.CheckPrivateOwnership(_pictogram, HttpContext);
                else if (_pictogram.AccessLevel == AccessLevel.PROTECTED)
                    ownsResource = await _giraf.CheckProtectedOwnership(_pictogram, HttpContext);

                if (ownsResource)
                    return Ok(new PictogramDTO(_pictogram, _pictogram.Image));
                else
                    return Unauthorized();
            } catch (Exception e)
            {
                string exceptionMessage = $"Exception occured in read:\n{e}";
                _giraf._logger.LogError(exceptionMessage);
                return BadRequest("There is most likely no pictograms available on the server.\n\n" + exceptionMessage);
            }
        }

        /// <summary>
        /// Create a new <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all relevant information about the new pictogram.</param>
        /// <returns>The new pictogram with all database-generated information.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePictogram(PictogramDTO pictogram)
        {
            if(pictogram == null) return BadRequest("The body of the request must contain a pictogram.");
            //Create the actual pictogram instance
            Pictogram pict = new Pictogram(pictogram.Title, pictogram.AccessLevel);
            pict.Image = pictogram.Image;

            var user = await _giraf.LoadUserAsync(HttpContext.User);

            if(pictogram.AccessLevel == AccessLevel.PRIVATE) {
                //Add the pictogram to the current user
                new UserResource(user, pict);
            }
            else if(pictogram.AccessLevel == AccessLevel.PROTECTED)
            {
                //Add the pictogram to the user's department
                new DepartmentResource(user.Department, pict);
            }

            //Stamp the pictogram with current time and add it to the database
            pict.LastEdit = DateTime.Now;
            await _giraf._context.Pictograms.AddAsync(pict);
            await _giraf._context.SaveChangesAsync();

            return Ok(new PictogramDTO(pict));
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
        [Authorize]
        public async Task<IActionResult> UpdatePictogramInfo([FromBody] PictogramDTO pictogram)
        {
            //Fetch the pictogram from the database and check that it exists
            var pict = await _giraf._context.Pictograms.Where(pic => pic.Id == pictogram.Id).FirstOrDefaultAsync();
            if(pict == null) return NotFound();

            bool ownsResource = false;
            switch (pict.AccessLevel)
            {
                case AccessLevel.PUBLIC:
                    ownsResource = true;
                    break;
                case AccessLevel.PROTECTED:
                    ownsResource = await _giraf.CheckProtectedOwnership(pict, HttpContext);
                    break;
                case AccessLevel.PRIVATE:
                    ownsResource = await _giraf.CheckPrivateOwnership(pict, HttpContext);
                    break;
            }
            if (!ownsResource)
                return Unauthorized();

            //Update the existing database entry and save the changes.
            pict.Merge(pictogram);
            pict.Image = pictogram.Image;
            _giraf._context.Pictograms.Update(pict);
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
            var pict = await _giraf._context.Pictograms.Where(pic => pic.Id == id).FirstOrDefaultAsync();
            if(pict == null) return NotFound();

            var ownsResource = false;
            switch (pict.AccessLevel)
            {
                case AccessLevel.PUBLIC:
                    ownsResource = true;
                    break;
                case AccessLevel.PROTECTED:
                    ownsResource = await _giraf.CheckProtectedOwnership(pict, HttpContext);
                    break;
                case AccessLevel.PRIVATE:
                    ownsResource = await _giraf.CheckPrivateOwnership(pict, HttpContext);
                    break;
            }
            if (!ownsResource)
                return Unauthorized();

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
        [Consumes("image/png", "image/jpeg")]
        [Authorize]
        public async Task<IActionResult> CreateImage(long id)
        {
            //Fetch the image and check that it exists
            var pict = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            if(pict == null) return NotFound();
            else if(pict.Image != null) return BadRequest("The pictogram already has an image.");

            var ownsPictogram = false;
            switch (pict.AccessLevel)
            {
                case AccessLevel.PUBLIC:
                    ownsPictogram = true;
                    break;
                case AccessLevel.PROTECTED:
                    ownsPictogram = await _giraf.CheckProtectedOwnership(pict, HttpContext);
                    break;
                case AccessLevel.PRIVATE:
                    ownsPictogram = await _giraf.CheckPrivateOwnership(pict, HttpContext);
                    break;
            }
            if (!ownsPictogram)
                return Unauthorized();

            //Read the image from the request body
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            if(image.Length == 0)
            {
                return BadRequest("The request contained no image.");
            }

            pict.Image = image;
            var pictoResult = await _giraf._context.SaveChangesAsync();

            return Ok(new PictogramDTO(pict, image));
        }

        /// <summary>
        /// Update the image of a <see cref="Pictogram"/> pictogram with the given Id.
        /// </summary>
        /// <param name="id">Id of the pictogram to update the image for.</param>
        /// <returns>The updated pictogram along with its image.</returns>
        [Consumes("image/png", "image/jpeg")]
        [HttpPut("image/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePictogramImage(long id) {
            //Attempt to fetch the pictogram from the database.
            var picto = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            if(picto == null) return NotFound();
            else if(picto.Image == null) return BadRequest("The pictogram does not have a image, please POST instead.");

            //Check ownership
            var ownsPictogram = false;
            switch (picto.AccessLevel)
            {
                case AccessLevel.PUBLIC:
                    ownsPictogram = true;
                    break;
                case AccessLevel.PROTECTED:
                    ownsPictogram = await _giraf.CheckProtectedOwnership(picto, HttpContext);
                    break;
                case AccessLevel.PRIVATE:
                    ownsPictogram = await _giraf.CheckPrivateOwnership(picto, HttpContext);
                    break;
            }
            if (!ownsPictogram)
                return Unauthorized();

            //Update the image
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            picto.Image = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(new PictogramDTO(picto, image));
        }

        /// <summary>
        /// Read the image of a given pictogram.
        /// </summary>
        /// <param name="id">The id of the pictogram to read the image of.</param>
        /// <returns>A FileResult with the desired image.</returns>
        [HttpGet("image/{id}")]
        public async Task<IActionResult> ReadPictogramImage(long id) {
            //Fetch the pictogram and check that it actually exists and has an image.
            var picto = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            if (picto == null)
                return NotFound($"There is no image with id {id}.");
            else if (picto.Image == null)
                return NotFound("The specified pictogram has no image.");

            //Check ownership
            var ownsPictogram = false;
            switch (picto.AccessLevel)
            {
                case AccessLevel.PUBLIC:
                    ownsPictogram = true;
                    break;
                case AccessLevel.PROTECTED:
                    ownsPictogram = await _giraf.CheckProtectedOwnership(picto, HttpContext);
                    break;
                case AccessLevel.PRIVATE:
                    ownsPictogram = await _giraf.CheckPrivateOwnership(picto, HttpContext);
                    break;
                default:
                    break;
            }
            if (!ownsPictogram)
                return Unauthorized();

            return File(picto.Image, "image/png");
        }
        #endregion

        #region helpers
        /// <summary>
        /// Read all pictograms available to the current user (or only the PUBLIC ones if no user is authorized).
        /// </summary>
        /// <returns>A list of said pictograms.</returns>
        private async Task<List<Pictogram>> ReadAllPictograms() {
            try
            {
                //Fetch all public pictograms and cask to a list - using Union'ing two IEnumerables gives an exception.
                var _pictograms = await _giraf._context.Pictograms
                    .Where(p => p.AccessLevel == AccessLevel.PUBLIC)
                    .ToListAsync();

                //Find the user and add his pictograms to the result
                var user = await _giraf.LoadUserAsync(HttpContext.User);
                if (user != null)
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
                    if (dep != null)
                    {
                        _giraf._logger.LogInformation($"Fetching pictograms for department {dep.Name}");
                        var depPictograms = dep.Resources
                            .Select(dr => dr.Resource)
                            .OfType<Pictogram>();
                        _pictograms = _pictograms
                            .Union(depPictograms)
                            .ToList();
                    }
                    else _giraf._logger.LogWarning($"{user.UserName} has no department.");
                }

                //Return the list of pictograms as Pictogram DTOs
                //- returning Pictograms directly causes an exception due to circular references
                return _pictograms;
            } catch (Exception e)
            {
                _giraf._logger.LogError("An exception occurred when reading all pictograms.", $"Message: {e.Message}", $"Source: {e.Source}");
                return null;
            }
        }
        #endregion
        #region query filters
        /// <summary>
        /// Filter a list of pictograms by their title.
        /// </summary>
        /// <param name="pictos">A list of pictograms that should be filtered.</param>
        /// <param name="titleQuery">The string that specifies what to search for.</param>
        /// <returns>A list of all pictograms with 'titleQuery' as substring.</returns>
        public List<Pictogram> FilterByTitle(List<Pictogram> pictos, string titleQuery) { 
            var matches = pictos
                .Where(p => p.Title.ToLower().Contains(titleQuery.ToLower()));

            return matches.ToList();
        }
        #endregion
    }
}