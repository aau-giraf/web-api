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
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The pictogram controller fetches an delivers pictograms on request. It also has endpoints for fetching
    /// and uploading images to pictograms. Supported image-types are .png and .jpg.
    /// </summary>
    [Route("v1/[controller]")]
    [Authorize]
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
        /// <param name="girafController"></param>
        /// <param name="lFactory"></param>
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
        /// <param name="query">The query string. pictograms are filtered based on this string if passed</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of pictograms per page</param>
        /// <returns> All the user's <see cref="Pictogram"/> pictograms.
        /// BadRequest if the request query was invalid, or if no pictograms were found
        /// </returns>
        [HttpGet("")]
        public async Task<Response<List<WeekPictogramDTO>>> ReadPictograms([FromQuery]string query, [FromQuery]int page = 1, [FromQuery]int pageSize = 10)
        {
            if(pageSize < 1 || pageSize > 100) 
                return new ErrorResponse<List<WeekPictogramDTO>>(ErrorCode.InvalidProperties, "pageSize must be in the range 1-100");
            if(page < 1)
                return new ErrorResponse<List<WeekPictogramDTO>>(ErrorCode.InvalidProperties, "page");
            //Produce a list of all pictograms available to the user
            var userPictograms = await ReadAllPictograms();
            if (userPictograms == null)
                return new ErrorResponse<List<WeekPictogramDTO>>(ErrorCode.PictogramNotFound);

            //Filter out all that does not satisfy the query string, if such is present.
            if(!String.IsNullOrEmpty(query)) 
                userPictograms = userPictograms.OrderBy((Pictogram _p) => IbsenDistance(query, _p.Title));

            return new Response<List<WeekPictogramDTO>>(await userPictograms.OfType<Pictogram>().Skip((page-1)*pageSize).Take(pageSize).Select(_p => new WeekPictogramDTO(_p)).ToListAsync());
        }

        private int IbsenDistance(string a, string b) {
            const int insertionCost = 1;
            const int deletionCost = 100;
            const int substitutionCost = 100;
            int[,] d = new int[a.Length+1,b.Length+1];
            for(int i = 0; i <= a.Length; i++)
                for(int j = 0; j <= b.Length; j++)
                    d[i,j] = 0;
            
            for(int i = 1; i <= a.Length; i++)
                d[i,0] = i*deletionCost;
            
            for(int j = 1; j <= b.Length; j++)
                d[0,j] = j*insertionCost;
            
            for(int j = 1; j <= b.Length; j++) {
                for(int i = 1; i <= a.Length; i++) {
                    int _substitutionCost = 0;
                    if(a[i-1] != b[j-1])
                        _substitutionCost = substitutionCost;

                    d[i,j] = Math.Min(d[i-1, j  ] + deletionCost,
                             Math.Min(d[i  , j-1] + insertionCost,
                                      d[i-1, j-1] + _substitutionCost));
                }
            }
            return d[a.Length,b.Length];
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
        public async Task<Response<WeekPictogramDTO>> ReadPictogram(long id)
        {
            try
            {
                //Fetch the pictogram and check that it actually exists
                var pictogram = await _giraf._context.Pictograms
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                if (pictogram == null) 
                    return new ErrorResponse<WeekPictogramDTO>(ErrorCode.PictogramNotFound);

                //Check if the pictogram is public and return it if so
                if (pictogram.AccessLevel == AccessLevel.PUBLIC) 
                    return new Response<WeekPictogramDTO>(new WeekPictogramDTO(pictogram));

                var usr = await _giraf.LoadUserAsync(HttpContext.User);
                if (usr == null) 
                    return new ErrorResponse<WeekPictogramDTO>(ErrorCode.UserNotFound);
                
                var ownsResource = false;
                if (pictogram.AccessLevel == AccessLevel.PRIVATE)
                    ownsResource = await _giraf.CheckPrivateOwnership(pictogram, usr);
                else if (pictogram.AccessLevel == AccessLevel.PROTECTED)
                    ownsResource = await _giraf.CheckProtectedOwnership(pictogram, usr);

                if (ownsResource)
                    return new Response<WeekPictogramDTO>(new WeekPictogramDTO(pictogram));
                else
                    return new ErrorResponse<WeekPictogramDTO>(ErrorCode.NotAuthorized);
            } catch (Exception e)
            {
                var exceptionMessage = $"Exception occured in read:\n{e}";
                _giraf._logger.LogError(exceptionMessage);
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.Error);
            }
        }

        /// <summary>
        /// Create a new <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all relevant information about the new pictogram.</param>
        /// <returns>The new pictogram with all database-generated information.
        /// BadRequest if some data was missing from either the PictogramDTO or the user</returns>
        [HttpPost("")]
        public async Task<Response<WeekPictogramDTO>> CreatePictogram([FromBody]PictogramDTO pictogram)
        {

            var user = await _giraf.LoadUserAsync(HttpContext.User);

            if (user == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.NotFound);

            if (pictogram == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, 
                    "Could not read pictogram DTO. Please make sure not to include image data in this request. " +
                    "Use POST localhost/v1/pictogram/{id}/image instead.");
            
            if (!ModelState.IsValid)
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.InvalidModelState);

            //Create the actual pictogram instance
            // if access level is not specified, missing properties
            if(pictogram.AccessLevel == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, "access level, pictogram");

            Pictogram pict =
                new Pictogram(pictogram.Title, (AccessLevel) pictogram.AccessLevel);

            if(pictogram.AccessLevel == AccessLevel.PRIVATE) {
                //Add the pictogram to the current user
                new UserResource(user, pict);
            }
            else if(pictogram.AccessLevel == AccessLevel.PROTECTED)
            {
                //Add the pictogram to the user's department
                new DepartmentResource(user.Department, pict);
            }

            await _giraf._context.Pictograms.AddAsync(pict);
            await _giraf._context.SaveChangesAsync();

            return new Response<WeekPictogramDTO>(new WeekPictogramDTO(pict));
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
        public async Task<Response<WeekPictogramDTO>> UpdatePictogramInfo(long id, [FromBody] PictogramDTO pictogram)
        {
            if (pictogram == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, "Could not read pictogram DTO. Please make sure not to include image data in this request. " +  "Use POST localhost/v1/pictogram/{id}/image instead.");
            if (pictogram.AccessLevel == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, "missing access level");
            if (!ModelState.IsValid)
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.InvalidModelState);

            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null) return new ErrorResponse<WeekPictogramDTO>(ErrorCode.NotFound);
            //Fetch the pictogram from the database and check that it exists
            var pict = await _giraf._context.Pictograms
                .Where(pic => pic.Id == id)
                .FirstOrDefaultAsync();
            if (pict == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.PictogramNotFound);

            if (!CheckOwnership(pict, usr).Result) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.NotAuthorized);
            //Ensure that Id is not changed.
            pictogram.Id = id;
            //Update the existing database entry and save the changes.
            pict.Merge(pictogram);
            _giraf._context.Pictograms.Update(pict);
            await _giraf._context.SaveChangesAsync();

            return new Response<WeekPictogramDTO>(new WeekPictogramDTO(pict));
        }

        /// <summary>
        /// Delete the <see cref="Pictogram"/> pictogram with the specified id.
        /// </summary>
        /// <param name="id">The id of the pictogram to delete.</param>
        /// <returns>Ok if the pictogram was deleted,
        /// NotFound if no pictogram with the id exists.
        /// Unauthorized if the user does not own the pictogram</returns>
        [HttpDelete("{id}")]
        public async Task<Response> DeletePictogram(int id)
        {
            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null) 
                return new ErrorResponse(ErrorCode.NotFound);
            //Fetch the pictogram from the database and check that it exists
            var pict = await _giraf._context.Pictograms.Where(pic => pic.Id == id).FirstOrDefaultAsync();
            if(pict == null) 
                return new ErrorResponse(ErrorCode.PictogramNotFound);

            if (!CheckOwnership(pict, usr).Result)
                return new ErrorResponse(ErrorCode.NotAuthorized);

            // Before we can remove a pictogram we must delete all its relations
            var userRessourceRelations = _giraf._context.UserResources.Where(ur => ur.PictogramKey == pict.Id);
            _giraf._context.UserResources.RemoveRange(userRessourceRelations);

            var depRessourceRelations = _giraf._context.DepartmentResources
                                              .Where(ur => ur.PictogramKey == pict.Id);
            _giraf._context.DepartmentResources.RemoveRange(depRessourceRelations);

            var weekDayRessourceRelations = _giraf._context.Activities
                                                  .Where(ur => ur.PictogramKey == pict.Id);
            _giraf._context.Activities.RemoveRange(weekDayRessourceRelations);

            await _giraf._context.SaveChangesAsync();

            // Now we can safely delete the pictogram
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
        public async Task<Response<WeekPictogramDTO>> SetPictogramImage(long id) {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            
            if (user == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.UserNotFound);
            
            //Attempt to fetch the pictogram from the database.
            var pictogram = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            
            if (pictogram == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.PictogramNotFound);

            if (!CheckOwnership(pictogram, user).Result)
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.NotAuthorized);

            //Update the image
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);

            if (image.Length == 0)
                pictogram.Image = null;
            else
                pictogram.Image = image;

            await _giraf._context.SaveChangesAsync();
            return new Response<WeekPictogramDTO>(new WeekPictogramDTO(pictogram));
        }

        /// <summary>
        /// Read the image of a given pictogram as raw.
        /// </summary>
        /// <param name="id">The id of the pictogram to read the image of.</param>
        /// <returns>A FileResult with the desired image.
        /// NotFound if the image does not exist
        /// Unauthorized if the user does not have access to it</returns>
        [HttpGet("{id}/image")]
        public async Task<Response<byte[]>> ReadPictogramImage(long id) {
            //Fetch the pictogram and check that it actually exists and has an image.
            var picto = await _giraf._context
                .Pictograms
                .Where(pictogram => pictogram.Id == id)
                .FirstOrDefaultAsync();
            if (picto == null)
                return new ErrorResponse<byte[]>(ErrorCode.PictogramNotFound);
            
            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null) 
                return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);

            if (picto.Image == null)
                return new ErrorResponse<byte[]>(ErrorCode.PictogramHasNoImage);

            if (!CheckOwnership(picto, usr).Result)
                return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);

            return new Response<byte[]>(picto.Image);
        }

        /// <summary>
        /// Reads the raw pictogram image.
        /// You are allowed to read all public pictograms aswell as your own pictograms
        ///  or any pictograms shared within the department
        /// </summary>
        /// <returns>The raw pictogram image.</returns>
        /// <param name="id">Identifier.</param>
        [HttpGet("{id}/image/raw")]
        public async Task<IActionResult> ReadRawPictogramImage(long id) {
            // fetch current authenticated user
            var usr = await _giraf.LoadUserAsync(HttpContext.User);
            if (usr == null)
                return NotFound();

            var picto = await _giraf._context
                .Pictograms
                .Where(pictogram => pictogram.Id == id)
                .FirstOrDefaultAsync();

            if (picto == null || picto.Image == null)
                return NotFound();

            // you can get all public pictograms
            if (picto.AccessLevel == AccessLevel.PUBLIC)
                return File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(picto.Image)), "image/png");

            // you can only get a protected picogram if it is owned by your department
            if (picto.AccessLevel == AccessLevel.PROTECTED && !picto.Departments.Any(d => d.OtherKey == usr.DepartmentKey))
                return NotFound();

            // you can only get a private pictogram if you are among the owners of the pictogram
            if (picto.AccessLevel == AccessLevel.PRIVATE && !picto.Users.Any(d => d.OtherKey == usr.Id))
                return NotFound();

            return File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(picto.Image)), "image/png");
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
                            .Where(pictogram => pictogram.AccessLevel == AccessLevel.PUBLIC 
                            //All the users pictograms
                            || pictogram.Users.Any(ur => ur.OtherKey == user.Id)
                            //All the department's pictograms
                            || pictogram.Departments.Any(dr => dr.OtherKey == user.DepartmentKey));
                    }

                    return _giraf._context.Pictograms.AsNoTracking()
                            //All public pictograms
                            .Where(pictogram => pictogram.AccessLevel == AccessLevel.PUBLIC 
                            //All the users pictograms
                            || pictogram.Users.Any(ur => ur.OtherKey == user.Id));
                }

                //Fetch all public pictograms as there is no user.
                return _giraf._context.Pictograms.AsNoTracking()
                    .Where(pictogram => pictogram.AccessLevel == AccessLevel.PUBLIC);
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
        private IQueryable<Pictogram> FilterByTitle(IQueryable<Pictogram> pictos, string titleQuery) { 
            return pictos
                .Where(pictogram => pictogram.Title.ToLower().Contains(titleQuery.ToLower()));
        }
        #endregion
    }
}