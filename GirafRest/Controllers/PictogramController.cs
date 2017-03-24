using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using Microsoft.Extensions.FileProviders;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.Extensions.Logging;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class PictogramController : Controller
    {
        public readonly GirafDbContext _context;
        public readonly UserManager<GirafUser> _userManager;
        public readonly IHostingEnvironment _env;
        private readonly ILogger _logger;

        public PictogramController(GirafDbContext context, UserManager<GirafUser> userManager, 
            IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this._context = context;
            this._userManager = userManager;
            this._env = env;
            this._logger = loggerFactory.CreateLogger<PictogramController>();
        }

        /// <summary>
        /// Get all public <see cref="Pictogram"/> pictograms and if authorized also his protected and private.
        /// </summary>
        /// <returns> All PUBLIC, PROTECTED and PRIVATE <see cref="Pictogram"/> pictograms available </returns>
        [HttpGet]
        public async Task<IActionResult> Read()
        {
            //Fetch all public pictograms
            var _pictograms = await _context.Pictograms
                .Where(p => p.AccessLevel == AccessLevel.PUBLIC)
                .ToListAsync();
            
            //Find the user and add his pictograms to the result
            var user = await LoadUserAsync(HttpContext.User);
            if(user != null)
            {
                _logger.LogInformation($"Fetching user pictograms for user {user.UserName}");
                var userPictograms = user.Resources.Select(ur => ur.Resource).OfType<Pictogram>();
                _pictograms = _pictograms.Union(userPictograms).ToList();
                //Also find his department and their pictograms
                var dep = user.Department;
                if(dep != null){
                    _logger.LogInformation($"Fetching pictograms for department {dep.Name}");
                    var depPictograms = dep.Resources.Select(dr => dr.Resource).OfType<Pictogram>();
                _pictograms = _pictograms.Union (depPictograms).ToList();
                }
                else _logger.LogWarning($"{user.UserName} has no department.");
            }

            return Ok(_pictograms.Select(p => new {p.Key, p.Title, p.AccessLevel, p.lastEdit}));
        }

        /// <summary>
        /// Get the image of a <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and check if the user is authorized if it is PRIVATE or PROTECTED.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> an image of a <see cref="Pictogram"/> pictogram with a specific <paramref name="id"/> id </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var _pictogram = await _context.Pictograms.Where(p => p.Key == id).FirstAsync();

            /*if (_pictogram == null && is_auth ?)
            {
                _pictogram = await _context.Pictograms.Where(p => p.Department.members.Contains(User) && p.Key == id);
            }*/
            return Ok(_pictogram);
        }

        /// <summary>
        /// Create a <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePictogram([FromBody] PictogramDTO pictogram)
        {
            //Find all involved entities
            Pictogram pict = new Pictogram(pictogram.Title, pictogram.AccessLevel);
            //Establish many-to-many relationship
            foreach(var usr in pictogram.Users) {
                new UserResource(usr, pict);
            }
            foreach (var dep in pictogram.Departments) {
                new DepartmentResource(dep, pict);
            }

            //Stamp the pictogram with current time and add it to the database, owner and department
            pict.lastEdit = DateTime.Now;
            var res = await _context.Pictograms.AddAsync(pict);

            _context.SaveChanges();

            return Ok(res.Entity);
        }

        /// <summary>
        /// Update info of a <see cref="Pictogram"/> pictogram with an id.
        /// </summary>
        /// <param name="pictogram">The pictogram to update the info for.</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdatePictogramInfo([FromBody] PictogramDTO pictogram)
        {
            var pict = await _context.Pictograms.Where(pic => pic.Key == pictogram.Id).FirstAsync();
            pict.Merge(pictogram);
            pict.lastEdit = DateTime.Now;
            var res = _context.Pictograms.Update(pict);
            return Ok(pict);
        }

        /// <summary>
        /// Delete a <see cref="Pictogram"/> pictogram with an id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePictogram(int id)
        {
            var pict = await _context.Pictograms.Where(pic => pic.Key == id).FirstAsync();

            /*var imageHandle = new FileInfo() //File.GetFileInfo(Path.Combine("images", $"{pict}.png"));
            if(imageHandle != null) {
                System.IO.File.Delete(imageHandle.PhysicalPath);
            }*/

            if(pict != null) {
                _context.Pictograms.Remove(pict);
                return Ok();
            }
            else
                return NotFound("There is no pictogram with the specified id.");
        }

        #region ImageHandling
        /// <summary>
        /// Get the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and check if the user is authorized if it is PRIVATE or PROTECTED.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> a <see cref="Pictogram"/> pictogram with a specific <paramref name="id"/> id </returns>
        [HttpGet]
        [Route("{id}/image")]
        [Produces("image/png")]
        public async Task<FileContentResult> ReadImage(int id)
        {
            string path = Path.Combine(_env.ContentRootPath, "images");

            if(!Directory.Exists(path)) {
                System.Console.WriteLine("Created a directory for images.");
                Directory.CreateDirectory(path);
            }

            FileInfo image = new FileInfo(Path.Combine(path, $"{id}.png"));
            if(!image.Exists) {
                return null;
            }

            byte[] imageBytes = new byte[image.Length];
            var fileReader = new FileStream(image.FullName, FileMode.Open);
            await fileReader.ReadAsync(imageBytes, 0, (int) image.Length, new CancellationToken());

            return base.File(imageBytes, "image/png");
        }

        /// <summary>
        /// Update image of a <see cref="Pictogram"/> pictogram with an id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/image")]
        [Consumes("image/png")]
        public async Task<IActionResult> CreatePictogramImage(long id)
        {
            //Copy body because of an unresolved issue with asp.net core
            System.Console.WriteLine(Request.Body);

            System.Console.WriteLine("Started PictogramImage controller - ready to post");
            /*var imageDir = _imageProvider.GetFileInfo("images");
            if(!Directory.Exists(imageDir.PhysicalPath)){
                Directory.CreateDirectory(imageDir.PhysicalPath);
                System.Console.WriteLine("Image directory created.");
            }

            System.Console.WriteLine("Reading request body");
            byte[] image = new byte[Request.Body.Length];
            await Request.Body.ReadAsync(image, 0, (int) Request.Body.Length);
            await Request.Body.FlushAsync();

            //Check if there exists a folder for images and create one if not
            System.Console.WriteLine("Adding pictogram image");
            //Add the actual image
            var imageFile = _imageProvider.GetFileInfo(Path.Combine("images", $"{id}.png"));
            if(imageFile.Exists)
                return BadRequest("An image for the specified pictogram already exists.");
            var imageStream = System.IO.File.Create(imageFile.PhysicalPath);
            await imageStream.WriteAsync(image, 0, image.Length);*/
            return Ok();
        }

        [Consumes("image/png")]
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdatePictogramImage(long id, [FromBody] byte[] image) {
            var picto = await _context.Pictograms.Where(p => p.Key == id).FirstAsync();

            //Delete the physical image and create a new from the image in the request.

            return Ok(picto);
        }
        #endregion

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
    }
}