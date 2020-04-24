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
using System.IO;
using GirafRest.Services;
using GirafRest.Models.Responses;
using Microsoft.Extensions.Hosting;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Manages pictograms, CRUD-ish.
    /// </summary>
    [Route("v1/[controller]")]
    [Authorize]
    public class PictogramController : Controller
    {
        private const string IMAGE_TYPE_PNG = "image/png";

        private readonly IGirafService _giraf;
        
        private readonly IHostEnvironment _hostingEnvironment;
        
        private readonly string imagePath;

        /// <summary>
        /// Constructor for controller
        /// </summary>
        /// <param name="girafController">Service Injection</param>
        /// <param name="lFactory">Service Injection</param>
        /// <param name="hostingEnvironment">Service Injection</param>
        public PictogramController(IGirafService girafController, ILoggerFactory lFactory, IHostEnvironment hostingEnvironment) 
    {
            _giraf = girafController;
            _giraf._logger = lFactory.CreateLogger("Pictogram");
            _hostingEnvironment = hostingEnvironment;
            imagePath = _hostingEnvironment.ContentRootPath + "/../pictograms/";
        }
        

        #region PictogramHandling
        /// <summary>
        /// Get all public <see cref="Pictogram"/> pictograms available to the user
        /// (i.e the public pictograms and those owned by the user (PRIVATE) and his department (PROTECTED)).
        /// </summary>
        /// <param name="query">The query string. pictograms are filtered based on this string if passed</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of pictograms per page</param>
        /// <returns> All the user's <see cref="Pictogram"/> pictograms on success else InvalidProperties or 
        /// PictogramNotFound </returns>
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

        /// <summary>
        /// Read the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and
        /// check if the user is authorized to see it.
        /// </summary>
        /// <param name="id">The id of the pictogram to fetch.</param>
        /// <returns> The <see cref="Pictogram"/> pictogram with the specified ID,
        /// NotFound  if no such <see cref="Pictogram"/> pictogram exists
        /// Else: PictogramNotFound, UserNotFound, Error, or NotAuthorized
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

                var usr = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
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
        /// Else: Notfound, MissingProperties or InvalidProperties </returns>
        [HttpPost("")]
        public async Task<Response<WeekPictogramDTO>> CreatePictogram([FromBody]PictogramDTO pictogram)
        {
            var user = await _giraf.LoadUserWithResources(HttpContext.User);

            if (user == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.NotFound);

            if (pictogram == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, 
                    "Could not read pictogram DTO. Please make sure not to include image data in this request. " +
                    "Use POST localhost/v1/pictogram/{id}/image instead.");
            
            if (!ModelState.IsValid)
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.InvalidProperties);

            //Create the actual pictogram instance
            // if access level is not specified, missing properties
            if(pictogram.AccessLevel == null || !Enum.IsDefined(typeof(AccessLevel), pictogram.AccessLevel)) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, "access level, pictogram");

            Pictogram pict =
                new Pictogram(pictogram.Title, (AccessLevel) pictogram.AccessLevel);

            if(pictogram.AccessLevel == AccessLevel.PRIVATE) {
                //Add relation between pictogram and current user 
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
        /// <param name="id">id in URL for Pictogram to update.</param>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all new information to update with.
        /// The Id found in this DTO is the target pictogram.
        /// </param>
        /// <returns> The updated Pictogram else: MissingProperties, InvalidProperties, NotFound, PictogramNotFound, or
        /// NotAuthorized </returns>
        [HttpPut("{id}")]
        public async Task<Response<WeekPictogramDTO>> UpdatePictogramInfo(long id, [FromBody] PictogramDTO pictogram)
        {
            if (pictogram == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, "Could not read pictogram DTO. Please make sure not to include image data in this request. " +  "Use POST localhost/v1/pictogram/{id}/image instead.");
            if (pictogram.AccessLevel == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.MissingProperties, "missing access level");
            if (!ModelState.IsValid)
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.InvalidProperties);

            var usr = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
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
        /// Else: UserNotFound,PictogramNotFound or NotAuthorized </returns>
        [HttpDelete("{id}")]
        public async Task<Response> DeletePictogram(int id)
        {
            var usr = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (usr == null) 
                return new ErrorResponse(ErrorCode.UserNotFound);
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
        /// Else: UserNotFound, PictogramNotFound or NotAuthorized</returns>
        
        [HttpPut("{id}/image")]
        public async Task<Response<WeekPictogramDTO>> SetPictogramImage(long id)
        {
            GirafUser user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            
            if (user == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.UserNotFound);
            
            //Attempt to fetch the pictogram from the database.
            Pictogram pictogram = await _giraf._context
                .Pictograms
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            
            if (pictogram == null) 
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.PictogramNotFound);

            if (!CheckOwnership(pictogram, user).Result)
                return new ErrorResponse<WeekPictogramDTO>(ErrorCode.NotAuthorized);
            
            //Update the image
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            
            // This sets the path that the system looks for when retrieving a pictogram
            string path = imagePath+ pictogram.Id + ".png";

            if (image.Length > 0){
                using (FileStream fs =
                    new FileStream(path,
                        FileMode.Create))
                {
                    
                    fs.Write(image);
                }

                pictogram.ImageHash = image.GetHashCode().ToString();
            }
            
            
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekPictogramDTO>(new WeekPictogramDTO(pictogram));
        }

        /// <summary>
        /// Read the image of a given pictogram as a sequence of bytes.
        /// </summary>
        /// <param name="id">The id of the pictogram to read the image of.</param>
        /// <returns>A The image else: PictogramNotFound, NotAuthorized, PictogramHasNoImage, 
        /// or NotAuthorized </returns>
        [HttpGet("{id}/image")]
        public async Task<Response<byte[]>> ReadPictogramImage(long id) {
            //Fetch the pictogram and check that it actually exists and has an image.
            var picto = await _giraf._context
                .Pictograms
                .Where(pictogram => pictogram.Id == id)
                .FirstOrDefaultAsync();
            if (picto == null)
                return new ErrorResponse<byte[]>(ErrorCode.PictogramNotFound);
            
            var usr = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (usr == null) 
                return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);

            if (picto.ImageHash == null)
                return new ErrorResponse<byte[]>(ErrorCode.PictogramHasNoImage);
            
            if (!CheckOwnership(picto, usr).Result)
                return new ErrorResponse<byte[]>(ErrorCode.NotAuthorized);

            return new Response<byte[]>(System.IO.File.ReadAllBytes(imagePath + picto.Id + ".png"));
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
            var usr = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (usr == null)
                return NotFound();

            var picto = await _giraf._context
                .Pictograms
                .Where(pictogram => pictogram.Id == id)
                .FirstOrDefaultAsync();

            if (picto == null || picto.ImageHash == null)
                return NotFound();
            
            // you can get all public pictograms
            if (CheckOwnership(picto, usr).Result)
            {
                _giraf._logger.LogInformation(imagePath);
                return PhysicalFile(imagePath + picto.Id + ".png", IMAGE_TYPE_PNG);
            }

            // you can only get a protected picogram if it is owned by your department
            if (picto.AccessLevel == AccessLevel.PROTECTED && !picto.Departments.Any(d => d.OtherKey == usr.DepartmentKey))
                return NotFound();

            // you can only get a private pictogram if you are among the owners of the pictogram
            if (picto.AccessLevel == AccessLevel.PRIVATE && !picto.Users.Any(d => d.OtherKey == usr.Id))
                return NotFound();

            return NotFound();
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

            return ownsPictogram;
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
                var user = await _giraf.LoadUserWithDepartment(HttpContext.User);
                
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

        /// <summary>
        /// The wagner-fisher implementation of the levenshtein distance named funny by my peers (long story)
        /// </summary>
        /// <returns>The edit distance between the strings a and b.</returns>
        /// <param name="a">Search string.</param>
        /// <param name="b">string to be compared against the search string</param>
        private int IbsenDistance(string a, string b)
        {
            const int insertionCost = 1;
            const int deletionCost = 100;
            const int substitutionCost = 100;
            int[,] d = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++)
                for (int j = 0; j <= b.Length; j++)
                    d[i, j] = 0;

            for (int i = 1; i <= a.Length; i++)
                d[i, 0] = i * deletionCost;

            for (int j = 1; j <= b.Length; j++)
                d[0, j] = j * insertionCost;

            for (int j = 1; j <= b.Length; j++)
            {
                for (int i = 1; i <= a.Length; i++)
                {
                    int _substitutionCost = 0;
                    if (a[i - 1] != b[j - 1])
                        _substitutionCost = substitutionCost;

                    d[i, j] = Math.Min(d[i - 1, j] + deletionCost,
                             Math.Min(d[i, j - 1] + insertionCost,
                                      d[i - 1, j - 1] + _substitutionCost));
                }
            }
            return d[a.Length, b.Length];
        }

        #endregion
    }
}
