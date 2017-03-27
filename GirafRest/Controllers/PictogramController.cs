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
        /// <summary>
        /// A reference to the database context - used to access the database and query for data. Handled by Asp.net's dependency injection.
        /// </summary>
        public readonly GirafDbContext _context;
        /// <summary>
        /// Asp.net's user manager. Can be used to fetch user data from the request's cookie. Handled by Asp.net's dependency injection.
        /// </summary>
        public readonly UserManager<GirafUser> _userManager;
        /// <summary>
        /// A reference to the hosting environment - somewhat like the Environment class in normal C# applications.
        /// It is used to find image files-paths. Handled by Asp.net's dependency injection.
        /// </summary>
        public readonly IHostingEnvironment _env;
        /// <summary>
        /// A data-logger used to write messages to the console. Handled by Asp.net's dependency injection.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// A constructor for the PictogramController. This is automatically called by Asp.net when receiving the first request for a pictogram.
        /// </summary>
        /// <param name="context">Reference to the database context.</param>
        /// <param name="userManager">Reference to Asp.net's user-manager.</param>
        /// <param name="env">Reference to an implementation of the IHostingEnvironment interface.</param>
        /// <param name="loggerFactory">Reference to an implementation of a logger.</param>
        public PictogramController(GirafDbContext context, UserManager<GirafUser> userManager, 
            IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this._context = context;
            this._userManager = userManager;
            this._env = env;
            this._logger = loggerFactory.CreateLogger<PictogramController>();
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
            var _pictogram = await _context.Pictograms.Where(p => p.Key == id).FirstAsync();
            if(_pictogram == null) return NotFound();

            //Attempt to read the image of the pictogram from a file
            byte[] imageBytes;
            try {
                imageBytes = await ReadImage(_pictogram.Key);
            }
            //Catch the exception that is thrown when the image-file is occupied by a writing process.
            catch (IOException) {
                return BadRequest("The server has not processed the image yet. Please wait a while and try again.");
            }

            //Check if the pictogram is public and return it if so
            if(_pictogram.AccessLevel == AccessLevel.PUBLIC) return Ok(new PictogramDTO(_pictogram, imageBytes));

            var ownsResource = await CheckForResourceOwnership(_pictogram);
            if(ownsResource) return Ok(new PictogramDTO(_pictogram, imageBytes));
            else return Unauthorized();
        }

        /// <summary>
        /// Create a new <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all relevant information about the new pictogram.</param>
        /// <returns>The new pictogram with all database-generated information.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePictogram([FromBody] PictogramDTO pictogram)
        {
            //Create the actual pictogram instance
            Pictogram pict = new Pictogram(pictogram.Title, pictogram.AccessLevel);

            var user = await LoadUserAsync(HttpContext.User);
            //Add the pictogram to the current user and his department
            new UserResource(user, pict);
            new DepartmentResource(user.Department, pict);

            //Stamp the pictogram with current time and add it to the database
            pict.LastEdit = DateTime.Now;
            var res = await _context.Pictograms.AddAsync(pict);
            await _context.SaveChangesAsync();

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
            var pict = await _context.Pictograms.Where(pic => pic.Key == pictogram.Id).FirstAsync();
            if(pict == null) return NotFound();

            //Update the existing database entry and save the changes.
            pict.Merge(pictogram);
            var res = _context.Pictograms.Update(pict);
            await _context.SaveChangesAsync();

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
            var pict = await _context.Pictograms.Where(pic => pic.Key == id).FirstAsync();
            if(pict == null) return NotFound();

            if(! await CheckForResourceOwnership(pict)) return Unauthorized();

            string imageDir = GetImageDirectory();
            var imageFile = new FileInfo(Path.Combine(imageDir, $"{id}.png"));
            if(imageFile.Exists) { 
                imageFile.Delete();
                _logger.LogInformation($"Deleted {imageFile.Name} from disc.");
            }

            //Remove it and save changes
            _context.Pictograms.Remove(pict);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #region ImageHandling
        /// <summary>
        /// Read the image of the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id from a file.
        /// </summary>
        /// <param name="id">Id of the pictogram to fetch image for.</param>
        /// <returns> A byte-array with the bytes of the image.</returns>
        public async Task<byte[]> ReadImage(long id)
        {
            string imageDir = GetImageDirectory();
            //Check if the image-file exists.
            FileInfo image = new FileInfo(Path.Combine(imageDir, $"{id}.png"));
            if(!image.Exists) {
                return null;
            }

            //Read the image from file into a byte array
            byte[] imageBytes = new byte[image.Length];
            var fileReader = new FileStream(image.FullName, FileMode.Open);
            await fileReader.ReadAsync(imageBytes, 0, (int) image.Length, new CancellationToken());

            return imageBytes;
        }

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
            var pict = await _context.Pictograms.Where(p => p.Key == id).FirstAsync();
            if(pict == null) return NotFound();

            string imageDir = GetImageDirectory();

            //Create image-file and copy the contents of the request-body into the new file
            var imageFile = new FileInfo(Path.Combine(imageDir, $"{id}.png"));
            if(imageFile.Exists)
                return BadRequest("An image for the specified pictogram already exists.");
            await WriteImage(HttpContext.Request.Body, imageFile);

            return Ok(new PictogramDTO(pict));
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
            var picto = await _context.Pictograms.Where(p => p.Key == id).FirstAsync();
            if(picto == null) return NotFound();

            string imageDir = GetImageDirectory();

            //Check if the file exists - if so delete it
            var imageInfo = new FileInfo(Path.Combine(imageDir, $"{id}.png"));
            if(imageInfo.Exists) imageInfo.Delete();
            await WriteImage(HttpContext.Request.Body, imageInfo);

            return Ok(new PictogramDTO(picto));
        }
        #endregion

        #region helpers
        /// <summary>
        /// Load the user from the <see cref="HttpContext"/> - both his information and all related data.
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> with related data.</returns>
        private async Task<GirafUser> LoadUserAsync(System.Security.Claims.ClaimsPrincipal principal)  {
            var usr = (await _userManager.GetUserAsync(principal));
            if(usr == null) return null;

            return await _context.Users
                    //First load the user from the database
                    .Where (u => u.Id == usr.Id)
                    //Then load his pictograms - both the relationship and the actual pictogram
                    .Include(u => u.Resources)
                    .ThenInclude(ur => ur.Resource)
                    //Then load his department and their pictograms
                    .Include(u => u.Department)
                    .ThenInclude(d => d.Resources)
                    .ThenInclude(dr => dr.Resource)
                    //And return him
                    .FirstAsync();
        }

        /// <summary>
        /// Get the path to the image directory, also checks if the directory for images exists and creates it if not.
        /// </summary>
        private string GetImageDirectory() {
            //Check that the image directory exists - create it if not.
            var imageDir = Path.Combine(_env.ContentRootPath, "images");
            if(!Directory.Exists(imageDir)){
                Directory.CreateDirectory(imageDir);
                _logger.LogInformation("Image directory created.");
            }

            return imageDir;
        }

        /// <summary>
        /// Copies the content of the request's body into the specified file.
        /// </summary>
        /// <param name="bodyStream">A byte-stream from the body of the request.</param>
        /// <param name="targetFile">The target file for the copy.</param>
        /// <returns>Actually nothing - Task return-type in order to make the method async.</returns>
        private async Task WriteImage(Stream bodyStream, FileInfo targetFile) {
            using (var imageStream = System.IO.File.Create(targetFile.FullName)) {
                await bodyStream.CopyToAsync(imageStream);
                await bodyStream.FlushAsync();
            }
        }

        /// <summary>
        /// Checks if the user owns the given <paramref name="pictogram"/> and returns true if so.
        /// Returns false if the user or his department does not own the <see cref="Pictogram"/>. 
        /// </summary>
        /// <param name="pictogram">The pictogram to check the ownership for.</param>
        /// <returns>True if the user is authorized to see the resource and false if not.</returns>
        private async Task<bool> CheckForResourceOwnership(Pictogram pictogram) {
            //The pictogram was not public, check if the user owns it.
            var usr = await LoadUserAsync(HttpContext.User);
            if(usr == null) return false;
            
            var ownedByUser = await _context.UserResources
                .Where(ur => ur.PictoFrameKey == pictogram.Key && ur.UserId == usr.Id)
                .AnyAsync();
            if(ownedByUser) return true;

            //The pictogram was not owned by user, check if his department owns it.
            var ownedByDepartment = await _context.DeparmentResources
                .Where(dr => dr.PictoFrameKey == pictogram.Key && dr.DeparmentKey == usr.DepartmentKey)
                .AnyAsync();
            if(ownedByDepartment) return true;

            return false;
        }

        private async Task<List<PictogramDTO>> ReadAllPictograms() {
            //Fetch all public pictograms and cask to a list - using Union'ing two IEnumerables gives an exception.
            var _pictograms = await _context.Pictograms
                .Where(p => p.AccessLevel == AccessLevel.PUBLIC)
                .ToListAsync();
            
            //Find the user and add his pictograms to the result
            var user = await LoadUserAsync(HttpContext.User);
            if(user != null)
            {
                _logger.LogInformation($"Fetching user pictograms for user {user.UserName}");
                var userPictograms = user.Resources
                    .Select(ur => ur.Resource)
                    .OfType<Pictogram>();
                _pictograms = _pictograms
                    .Union(userPictograms)
                    .ToList();
                //Also find his department and their pictograms
                var dep = user.Department;
                if(dep != null){
                    _logger.LogInformation($"Fetching pictograms for department {dep.Name}");
                    var depPictograms = dep.Resources
                        .Select(dr => dr.Resource)
                        .OfType<Pictogram>();
                _pictograms = _pictograms
                    .Union (depPictograms)
                    .ToList();
                }
                else _logger.LogWarning($"{user.UserName} has no department.");
            }
            
            //Return the list of pictograms as Pictogram DTOs
            //- returning Pictograms directly causes an exception due to circular references
            return _pictograms.Select(p => new PictogramDTO(p)).ToList();
        }
        #endregion
        #region query filters
        public List<PictogramDTO> FilterByTitle(List<PictogramDTO> pictos, string titleQuery) { 
            var mathces = pictos.Where(p => p.Title.ToLower().Contains(titleQuery.ToLower()));
            return mathces.ToList();
        }
        #endregion
    }
}