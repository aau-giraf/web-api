using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using GirafRest.Models.DTOs;
using GirafRest.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using GirafRest.Services;
using GirafRest.Extensions;
using GirafRest.Models.Responses;
using System.IO;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The pictogram controller fetches an delivers pictograms on request. It also has endpoints for fetching
    /// and uploading images to pictograms. Supported image-types are .png and .jpg.
    /// </summary>
    [Route("v1/[controller]")]
    public class PictogramController : Controller
    {
        /// <summary>
        /// Consts for image types.
        /// </summary>
        private const string IMAGE_TYPE_PNG = "image/png";
        private const string IMAGE_TYPE_JPEG = "image/jpeg";

        /// <summary>
        /// A reference to GirafService, that defines common functionality for all classes.
        /// </summary>
        private readonly IGirafService _giraf;

        /// <summary>
        /// Constructor for the Pictogram-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf">A reference to the GirafService.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a logger.</param>
        public PictogramController(IGirafService girafController, ILoggerFactory lFactory) 
        {
            _giraf = girafController;
            _giraf._logger = lFactory.CreateLogger("Pictogram");
        }

        #region PictogramHandling
        /// <summary>
        /// Get all public <see cref="Pictogram"/> pictograms available to the user
        /// (i.e the public pictograms and those owned by the user (PRIVATE) and his department (PROTECTED)).
        /// </summary>
        /// <param name="q">The query string. pictograms are filtered based on this string if passed</param>
        /// <param name="p">Page number</param>
        /// <param name="n">Number of pictograms per page, defaults to 10</param>
        /// <returns> All the user's <see cref="Pictogram"/> pictograms.
        /// BadRequest if the request query was invalid, or if no pictograms were found
        /// </returns>
        [HttpGet("")]
        public async Task<Response<List<PictogramDTO>>> ReadPictograms([FromQuery]string q, [FromQuery]int p = 0, [FromQuery]int n = 10)
        {
            //Produce a list of all pictograms available to the user
            var userPictograms = await ReadAllPictograms();
            if (userPictograms == null)
                return new ErrorResponse<List<PictogramDTO>>(ErrorCode.PictogramNotFound);

            System.Console.WriteLine($"GET ALL THE PICTOGRAMS WITH QUERY {q}");
            //Filter out all that does not satisfy the query string, if such is present.
            if(!String.IsNullOrEmpty(q)) 
                userPictograms = userPictograms.OrderBy((Pictogram _p) => IbsenDistance(q, _p.Title));

            return new Response<List<PictogramDTO>>(await userPictograms.OfType<Pictogram>().
                                                Skip(p*n).Take(n).Select(_p => new PictogramDTO(_p)).ToListAsync());
        }

        private int IbsenDistance(string a, string b) {
            return IbsenDistance(a,b,a.Length,b.Length);
        }
        private int IbsenDistance(string a, string b, int aLen, int bLen) {
            const int insertCost = 1;
            const int deleteCost = 4;
            const int substituteCost = 2;
            if(aLen <= 0 || bLen <= 0) return Math.Max(aLen, bLen)*insertCost;
            return Math.Min(IbsenDistance(a,b, aLen-1, bLen) +deleteCost,
                   Math.Min(IbsenDistance(a,b, aLen,   bLen-1) +insertCost,
                           IbsenDistance(a,b, aLen-1, bLen-1) +(a[aLen-1] == b[bLen-1] ? 0 : substituteCost))
            );
        }

        /// <summary>
        /// Read the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and
        /// check if the user is authorized to see it.
        /// </summary>
        /// <param name="id">The id of the pictogram to fetch.</param>
        /// <returns> The <see cref="Pictogram"/> pictogram with the specified ID,
        /// NotFound  if no such <see cref="Pictogram"/> pictogram exists,
        /// BadRequest if the <see cref="Pictogram"/> was not succesfully uploaded to the server or
        /// Unauthorized if the pictogram is private and user does not own it.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<Response<PictogramDTO>> ReadPictogram(long id)
        {
            try
            {
                //Fetch the pictogram and check that it actually exists
                var _pictogram = await _giraf._context.Pictograms
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                if (_pictogram == null) return new ErrorResponse<PictogramDTO>(ErrorCode.PictogramNotFound);

                //Check if the pictogram is public and return it if so
                if (_pictogram.AccessLevel == AccessLevel.PUBLIC) return new Response<PictogramDTO>(new PictogramDTO(_pictogram, _pictogram.Image));

                var usr = await _giraf.LoadUserAsync(HttpContext.User);
                if (usr == null) return new ErrorResponse<PictogramDTO>(ErrorCode.UserNotFound);
                
                bool ownsResource = false;
                if (_pictogram.AccessLevel == AccessLevel.PRIVATE)
                    ownsResource = await _giraf.CheckPrivateOwnership(_pictogram, usr);
                else if (_pictogram.AccessLevel == AccessLevel.PROTECTED)
                    ownsResource = await _giraf.CheckProtectedOwnership(_pictogram, usr);

                if (ownsResource)
                    return new Response<PictogramDTO>(new PictogramDTO(_pictogram, _pictogram.Image));
                else
                    return new ErrorResponse<PictogramDTO>(ErrorCode.NotAuthorized);
            } catch (Exception e)
            {
                string exceptionMessage = $"Exception occured in read:\n{e}";
                _giraf._logger.LogError(exceptionMessage);
                return new ErrorResponse<PictogramDTO>(ErrorCode.Error);
            }
        }

        /// <summary>
        /// Create a new <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all relevant information about the new pictogram.</param>
        /// <returns>The new pictogram with all database-generated information.
        /// BadRequest if some data was missing from either the PictogramDTO or the user</returns>
        [HttpPost("")]
        [Authorize]
        public async Task<Response<PictogramDTO>> CreatePictogram([FromBody]PictogramDTO pictogram)
        {
            if (pictogram == null) return new ErrorResponse<PictogramDTO>(ErrorCode.MissingProperties, "pictogram");
                BadRequest("The body of the request must contain a pictogram.");
            if (!ModelState.IsValid)
                return new ErrorResponse<PictogramDTO>(ErrorCode.InvalidModelState);

            //Create the actual pictogram instance
            // if access level is not specified, missing properties
            if(pictogram.AccessLevel == null) return new ErrorResponse<PictogramDTO>(ErrorCode.MissingProperties, "access level, pictogram");

            Pictogram pict = new Pictogram(pictogram.Title, (AccessLevel) pictogram.AccessLevel);
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

            return new Response<PictogramDTO>(new PictogramDTO(pict));
        }

        /// <summary>
        /// Update info of a <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all new information to update with.
        /// The Id found in this DTO is the target pictogram.
        /// </param>
        /// <returns> BadRequest if the PictogramDTO or user were invalid
        /// Unauthorized if the user does not own the Pictogram he is attempting to update
        /// NotFound if there is no pictogram with the specified id or 
        /// the updated pictogram to maintain statelessness.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = GirafRole.RequireGuardianOrSuperUser)]
        public async Task<Response<PictogramDTO>> UpdatePictogramInfo(long id, [FromBody] PictogramDTO pictogram)
        {
            if (pictogram == null) return new ErrorResponse<PictogramDTO>(ErrorCode.MissingProperties, "pictogram");
            if (pictogram.AccessLevel == null) return new ErrorResponse<PictogramDTO>(ErrorCode.MissingProperties, "missing access level");

            if (!ModelState.IsValid)
            {
                return new ErrorResponse<PictogramDTO>(ErrorCode.InvalidModelState);
            }

            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null) return new ErrorResponse<PictogramDTO>(ErrorCode.NotAuthorized);
            //Fetch the pictogram from the database and check that it exists
            var pict = await _giraf._context.Pictograms
                .Where(pic => pic.Id == id)
                .FirstOrDefaultAsync();
            if (pict == null) return new ErrorResponse<PictogramDTO>(ErrorCode.PictogramNotFound);

            if (!CheckOwnership(pict, usr).Result) return new ErrorResponse<PictogramDTO>(ErrorCode.NotAuthorized);
            //Ensure that Id is not changed.
            pictogram.Id = id;
            //Update the existing database entry and save the changes.
            pict.Merge(pictogram);
            _giraf._context.Pictograms.Update(pict);
            await _giraf._context.SaveChangesAsync();

            return new Response<PictogramDTO>(new PictogramDTO(pict));
        }

        /// <summary>
        /// Delete the <see cref="Pictogram"/> pictogram with the specified id.
        /// </summary>
        /// <param name="id">The id of the pictogram to delete.</param>
        /// <returns>Ok if the pictogram was deleted,
        /// NotFound if no pictogram with the id exists.
        /// Unauthorized if the user does not own the pictogram</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = GirafRole.RequireGuardianOrSuperUser)]
        public async Task<Response> DeletePictogram(int id)
        {
            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null) return new ErrorResponse(ErrorCode.UserNotFound);
            //Fetch the pictogram from the database and check that it exists
            var pict = await _giraf._context.Pictograms.Where(pic => pic.Id == id).FirstOrDefaultAsync();
            if(pict == null) return new ErrorResponse(ErrorCode.PictogramNotFound);

            if (!CheckOwnership(pict, usr).Result)
                return new ErrorResponse(ErrorCode.NotAuthorized);

            //Remove it and save changes
            _giraf._context.Pictograms.Remove(pict);
            await _giraf._context.SaveChangesAsync();
            return new Response();
        }
        #endregion
        #region ImageHandling

        /// <summary>
        /// Update the image of a <see cref="Pictogram"/> pictogram with the given Id.
        /// </summary>
        /// <param name="id">Id of the pictogram to update the image for.</param>
        /// <returns>The updated pictogram along with its image.
        /// Unauthorized if the user does not own the Pictogram,
        /// BadRequest if the Pictogram does not have an image yet
        /// NotFound if it does not exist</returns>
        [Consumes(IMAGE_TYPE_PNG, IMAGE_TYPE_JPEG)]
        [HttpPut("{id}/image")]
        [Authorize]
        public async Task<Response<PictogramDTO>> SetPictogramImage(long id) {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            
            if (user == null) 
                return new ErrorResponse<PictogramDTO>(ErrorCode.UserNotFound);
            
            //Attempt to fetch the pictogram from the database.
            var pictogram = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            
            if (pictogram == null) 
                return new ErrorResponse<PictogramDTO>(ErrorCode.PictogramNotFound);

            if (!CheckOwnership(pictogram, user).Result)
                return new ErrorResponse<PictogramDTO>(ErrorCode.NotAuthorized);

            //Update the image
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);

            if (image.Length == 0)
                pictogram.Image = null;
            else
                pictogram.Image = image;

            await _giraf._context.SaveChangesAsync();
            return new Response<PictogramDTO>(new PictogramDTO(pictogram, image));
        }

        /// <summary>
        /// Read the image of a given pictogram.
        /// </summary>
        /// <param name="id">The id of the pictogram to read the image of.</param>
        /// <returns>A FileResult with the desired image.
        /// NotFound if the image does not exist
        /// Unauthorized if the user does not have access to it</returns>
        [HttpGet("{id}/image")]
        public async Task<Response<byte[]>> ReadPictogramImage(long id) {
            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null) return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);
            //Fetch the pictogram and check that it actually exists and has an image.
            var picto = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            if (picto == null)
                return new ErrorResponse<byte[]>(ErrorCode.PictogramNotFound);
            else if (picto.Image == null)
                return new ErrorResponse<byte[]>(ErrorCode.PictogramHasNoImage);

            if (!CheckOwnership(picto, usr).Result)
                return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);

            return new Response<byte[]>(picto.Image);
        }

        /// <summary>
        /// Returns the pictogram image as a png file
        /// </summary>
        /// <param name="id">The id of the pictogram to read the image of.</param>
        /// <returns>The desired image as a PNG file.
        /// NotFound if the image does not exist
        /// Unauthorized if the user does not have access to it</returns>
        [HttpGet("{id}/image/raw")]
        public async Task<IActionResult> ReadRawPictogramImage(long id) {
            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null) 
                return NotFound(); 
                // return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);
            //Fetch the pictogram and check that it actually exists and has an image.
            var picto = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            if (picto == null)
                return NotFound(); 
                // return new ErrorResponse<byte[]>(ErrorCode.PictogramNotFound);
            else if (picto.Image == null)
                return NotFound(); 
                // return new ErrorResponse<byte[]>(ErrorCode.PictogramHasNoImage);

            if (!CheckOwnership(picto, usr).Result)
                return NotFound(); 
                // return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);

            return File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(picto.Image)), "image/png");
            
            // return File(SixLabors.ImageSharp.Image.FromStream(new MemoryStream(picto.Image)), "image/png");
            // return new Response<byte[]>(picto.Image);
        }
        #endregion

        #region helpers
        
        /// <summary>
        /// Checks if the user has some form of ownership of the pictogram.
        /// </summary>
        /// <param name="picto">The Pictogram in need of checking.</param>
        /// <param name="usr">The user in question.</param>
        /// <returns>A bool indicating whether the user owns the pictogram or not.</returns>
        private async Task<bool> CheckOwnership(Pictogram picto, GirafUser usr)
        {
            var ownsPictogram = false;
            switch (picto.AccessLevel)
            {
                case AccessLevel.PUBLIC:
                    ownsPictogram = true;
                    break;
                case AccessLevel.PROTECTED:
                    ownsPictogram = await _giraf.CheckProtectedOwnership(picto, usr);
                    break;
                case AccessLevel.PRIVATE:
                    ownsPictogram = await _giraf.CheckPrivateOwnership(picto, usr);
                    break;
                default:
                    break;
            }
            if (!ownsPictogram)
                return false;
            return true;
        }

        /// <summary>
        /// Read all pictograms available to the current user (or only the PUBLIC ones if no user is authorized).
        /// </summary>
        /// <returns>A list of said pictograms.</returns>
        private async Task<IQueryable<Pictogram>> ReadAllPictograms() {
            //In this method .AsNoTracking is used due to a bug in EntityFramework Core, where we are not allowed to call a constructor in .Select,
            //i.e. convert the pictograms to PictogramDTOs.
            try
            {
                //Find the user and add his pictograms to the result
                var user = await _giraf.LoadUserAsync(HttpContext.User);
                
                if (user != null)
                {
                    if (user.Department != null)
                    {
                        _giraf._logger.LogInformation($"Fetching pictograms for department {user.Department.Name}");
                        return _giraf._context.Pictograms.AsNoTracking()
                            //All public pictograms
                            .Where(p => p.AccessLevel == AccessLevel.PUBLIC 
                            //All the users pictograms
                            || p.Users.Any(ur => ur.OtherKey == user.Id)
                            //All the department's pictograms
                            || p.Departments.Any(dr => dr.OtherKey == user.DepartmentKey));
                    }

                    return _giraf._context.Pictograms.AsNoTracking()
                            //All public pictograms
                            .Where(p => p.AccessLevel == AccessLevel.PUBLIC 
                            //All the users pictograms
                            || p.Users.Any(ur => ur.OtherKey == user.Id));
                }

                //Fetch all public pictograms as there is no user.
                return _giraf._context.Pictograms.AsNoTracking()
                    .Where(p => p.AccessLevel == AccessLevel.PUBLIC);
            } catch (Exception e)
            {
                _giraf._logger.LogError("An exception occurred when reading all pictograms.", $"Message: {e.Message}", $"Source: {e.Source}");
                return null;
            }
        }
        /// <summary>
        /// Helper method for parsing queries involving integers
        /// </summary>
        /// <param name="queryStringName">The string to look for in the request</param>
        /// <param name="fallbackValue">The value to return should the request not contain a query</param>
        /// <returns>Either the fallback value, or the Integer found in the query</returns>
        private int parseQueryInteger(string queryStringName, int fallbackValue)
        {
            if (string.IsNullOrEmpty(HttpContext.Request.Query[queryStringName]))
                return fallbackValue;
            return int.Parse(HttpContext.Request.Query[queryStringName]);
        }
        #endregion
        #region query filters
        /// <summary>
        /// Filter a list of pictograms by their title.
        /// </summary>
        /// <param name="pictos">A list of pictograms that should be filtered.</param>
        /// <param name="titleQuery">The string that specifies what to search for.</param>
        /// <returns>A list of all pictograms with 'titleQuery' as substring.</returns>
        private IQueryable<Pictogram> FilterByTitle(IQueryable<Pictogram> pictos, string titleQuery) { 
            return pictos
                .Where(p => p.Title.ToLower().Contains(titleQuery.ToLower()));
        }
        #endregion
    }
}