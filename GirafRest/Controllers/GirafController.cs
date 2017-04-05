using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading;

namespace GirafRest.Controllers
{
    public class GirafController : Controller
    {
        /// <summary>
        /// A reference to the database context - used to access the database and query for data. Handled by Asp.net's dependency injection.
        /// </summary>
        protected readonly GirafDbContext _context;
        /// <summary>
        /// Asp.net's user manager. Can be used to fetch user data from the request's cookie. Handled by Asp.net's dependency injection.
        /// </summary>
        protected readonly UserManager<GirafUser> _userManager;
        /// <summary>
        /// A reference to the hosting environment - somewhat like the Environment class in normal C# applications.
        /// It is used to find image files-paths. Handled by Asp.net's dependency injection.
        /// </summary>
        protected readonly IHostingEnvironment _env;
        /// <summary>
        /// A data-logger used to write messages to the console. Handled by Asp.net's dependency injection.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// A constructor for the PictogramController. This is automatically called by Asp.net when receiving the first request for a pictogram.
        /// </summary>
        /// <param name="context">Reference to the database context.</param>
        /// <param name="userManager">Reference to Asp.net's user-manager.</param>
        /// <param name="env">Reference to an implementation of the IHostingEnvironment interface.</param>
        /// <param name="loggerFactory">Reference to an implementation of a logger.</param>
        public GirafController(GirafDbContext context, UserManager<GirafUser> userManager,
            IHostingEnvironment env, ILogger logger)
        {
            this._context = context;
            this._userManager = userManager;
            this._env = env;
            this._logger = logger;
        }
        
        /// <summary>
        /// Load the user from the <see cref="HttpContext"/> - both his information and all related data.
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> with related data.</returns>
        protected async Task<GirafUser> LoadUserAsync(System.Security.Claims.ClaimsPrincipal principal)  {
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
                    // then load his week schedule
                    .Include(u => u.WeekSchedule)
                    .ThenInclude(w => w.Days)
                    //And return him
                    .FirstAsync();
        }

        /// <summary>
        /// Read the image of the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id from a file.
        /// </summary>
        /// <param name="id">Id of the pictogram to fetch image for.</param>
        /// <returns> A byte-array with the bytes of the image.</returns>
        protected async Task<byte[]> ReadImage(long id)
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
        /// Get the path to the image directory, also checks if the directory for images exists and creates it if not.
        /// </summary>
        protected string GetImageDirectory() {
            //Check that the image directory exists - create it if not.
            var imageDir = Path.Combine(_env.ContentRootPath, "images");
            if(!Directory.Exists(imageDir)){
                Directory.CreateDirectory(imageDir);
                _logger.LogInformation("Image directory created.");
            }

            return imageDir;
        }

        /// <summary>
        /// Checks if the user owns the given <paramref name="pictogram"/> and returns true if so.
        /// Returns false if the user or his department does not own the <see cref="Pictogram"/>. 
        /// </summary>
        /// <param name="pictogram">The pictogram to check the ownership for.</param>
        /// <returns>True if the user is authorized to see the resource and false if not.</returns>
        protected async Task<bool> CheckForResourceOwnership(Frame pictogram) {
            //The pictogram was not public, check if the user owns it.
            var usr = await LoadUserAsync(HttpContext.User);
            if(usr == null) return false;
            
            var ownedByUser = await _context.UserResources
                .Where(ur => ur.ResourceKey == pictogram.Id && ur.OtherKey == usr.Id)
                .AnyAsync();
            if(ownedByUser) return true;

            //The pictogram was not owned by user, check if his department owns it.
            var ownedByDepartment = await _context.DepartmentResources
                .Where(dr => dr.ResourceKey == pictogram.Id && dr.OtherKey == usr.DepartmentKey)
                .AnyAsync();
            if(ownedByDepartment) return true;

            return false;
        }
    }
}
