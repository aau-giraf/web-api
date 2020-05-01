using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GirafRest.Services
{
    /// <summary>
    /// The GirafService class implements the <see cref="IGirafService"/> interface and thus implements common
    /// functionality that is needed by most controllers.
    /// </summary>
    public class GirafService : IGirafService
    {
        /// <summary>
        /// A reference to the database context - used to access the database and query for data. Handled by asp.net's dependency injection.
        /// </summary>
        public GirafDbContext _context { get;  }
        /// <summary>
        /// Asp.net's user manager. Can be used to fetch user data from the request's cookie. Handled by asp.net's dependency injection.
        /// </summary>
        public UserManager<GirafUser> _userManager { get;  }
        /// <summary>
        /// A data-logger used to write messages to the console. Handled by asp.net's dependency injection.
        /// </summary>
        public ILogger _logger { get; set; }

        /// <summary>
        /// The most general constructor for GirafService. This constructor is used by both the other constructors and the unit tests.
        /// </summary>
        /// <param name="context">Reference to the database context.</param>
        /// <param name="userManager">Reference to asp.net's user-manager.</param>
        public GirafService(GirafDbContext context, UserManager<GirafUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        /// <summary>
        /// Method for loading user from context and eager loading <b>resources</b> fields
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithResources(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var usr = (await _userManager.GetUserAsync(principal));
            if (usr == null) return null;
            return await _context.Users
                //Get user by ID from database
                .Where(u => u.Id == usr.Id)
                //Then load his pictograms - both the relationship and the actual pictogram
                .Include(u => u.Resources)
                    .ThenInclude(ur => ur.Pictogram)
                //Then load his department and their pictograms
                .Include(u => u.Department)
                    .ThenInclude(d => d.Resources)
                        .ThenInclude(dr => dr.Pictogram)
                //And return it
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Method for loading user from context and eager loading <b>resources</b> fields
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithDepartment(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var usr = (await _userManager.GetUserAsync(principal));
            if (usr == null) return null;
            return await _context.Users
                .Where(u => u.Id == usr.Id)
                .Include(u => u.Department)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Method for loading user from context and eager loading fields requied to read their <b>week schedules</b>
        /// </summary>
        /// <param name="id">id of user to load.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithWeekSchedules(string id){
            var user = await _context.Users
                //First load the user from the database
                .Where(u => u.Id.ToLower() == id.ToLower())
                // then load his week schedule
                .Include(u => u.WeekSchedule)
                .ThenInclude(w => w.Thumbnail)
                .Include(u => u.WeekSchedule)
                .ThenInclude(w => w.Weekdays)
                .ThenInclude(wd => wd.Activities)
                .ThenInclude(e => e.Pictogram)
                //And return it
                .FirstOrDefaultAsync();

            return user;
        }

        /// <summary>
        /// Method for loading user from context, but including no fields. No reference types will be available.
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> without any related data.</returns>
        public async Task<GirafUser> LoadBasicUserDataAsync(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var usr = (await _userManager.GetUserAsync(principal));
            if(usr == null) return null;
            return await _context.Users
                //Get user by ID from database
                .Where(u => u.Id == usr.Id)
                //And return it
                .FirstOrDefaultAsync();;
        }

        /// <summary>
        /// Reads an image from the current request's body and return it as a byte array.
        /// </summary>
        /// <param name="bodyStream">A byte-stream from the body of the request.</param>
        /// <returns>The image found in the request represented as a byte array.</returns>
        public async Task<byte[]> ReadRequestImage(Stream bodyStream)
        {
            byte[] image;
            using (var imageStream = new MemoryStream())
            {
                await bodyStream.CopyToAsync(imageStream);

                try      //I assume this will always throw, but I dare not remove it, because why would it be here?
                {
                    await bodyStream.FlushAsync();
                }
                catch (NotSupportedException)
                {
                }
                
                image = imageStream.ToArray();
            }

            return image;
        }

        /// <summary>
        /// Checks if the user owns the given <paramref name="pictogram"/>.
        /// </summary>
        /// <param name="pictogram">The pictogram to check the ownership for.</param>
        /// <param name="user"></param>
        /// <returns>True if the user is authorized to see the resource and false if not.</returns>
        public async Task<bool> CheckPrivateOwnership(Pictogram pictogram, GirafUser user) {
            if (pictogram.AccessLevel != AccessLevel.PRIVATE)
                return false;

            //The pictogram was not public, check if the user owns it.
            if(user == null) return false;
            var ownedByUser = await _context.UserResources
                .Where(ur => ur.PictogramKey == pictogram.Id && ur.OtherKey == user.Id)
                .AnyAsync();

            return ownedByUser;
        }

        /// <summary>
        /// Checks if the current user's department owns the given resource.
        /// </summary>
        /// <param name="resource">The resource to check ownership for.</param>
        /// <param name="user"></param>
        /// <returns>True if the user's department owns the pictogram, false if not.</returns>
        public async Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user)
        {
            if (resource.AccessLevel != AccessLevel.PROTECTED)
                return false;

            if (user == null) return false;

            //The pictogram was not owned by user, check if his department owns it.
            var ownedByDepartment = await _context.DepartmentResources
                .Where(dr => dr.PictogramKey == resource.Id && dr.OtherKey == user.Department.Key)
                .AnyAsync();
                
            return ownedByDepartment;
        }
    }
}
