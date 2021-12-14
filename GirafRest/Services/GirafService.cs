using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Repositories;
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
        private readonly IGirafUserRepository _girafUserRepository;
        private readonly IUserResourseRepository _userResourseRepository;
        private readonly IDepartmentResourseRepository _departmentResourseRepository;
        /// <summary>
        /// A reference to the database context - used to access the database and query for data. Handled by asp.net's dependency injection.
        /// </summary>
        public GirafDbContext _context { get; }
        /// <summary>
        /// Asp.net's user manager. Can be used to fetch user data from the request's cookie. Handled by asp.net's dependency injection.
        /// </summary>
        public UserManager<GirafUser> _userManager { get; set; }
        /// <summary>
        /// A data-logger used to write messages to the console. Handled by asp.net's dependency injection.
        /// </summary>
        public ILogger _logger { get; set; }

        /// <summary>
        /// The most general constructor for GirafService. This constructor is used by both the other constructors and the unit tests.
        /// </summary>
        /// <param name="userManager">Reference to asp.net's user-manager.</param>
        /// <param name="girafUserRepository">Service Injection</param>
        /// <param name="userResourseRepository">Service Injection</param>
        /// <param name="departmentResourseRepository">Service Injection</param>
        public GirafService(UserManager<GirafUser> userManager,
            IGirafUserRepository girafUserRepository,
            IUserResourseRepository userResourseRepository,
            IDepartmentResourseRepository departmentResourseRepository)
        {
            _userManager = userManager;
            _girafUserRepository = girafUserRepository;
            _userResourseRepository = userResourseRepository;
            _departmentResourseRepository = departmentResourseRepository;
        }
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
        /// Method for loading user from context and eager loading <b>resources</b> fields
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithResources(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var usr = (await _userManager.GetUserAsync(principal));
            if (usr == null) return null;
            return await _girafUserRepository.LoadUserWithResources(usr);
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
            return await _girafUserRepository.LoadUserWithDepartment(usr);
        }

        /// <summary>
        /// Method for loading user from context and eager loading fields requied to read their <b>week schedules</b>
        /// </summary>
        /// <param name="id">id of user to load.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithWeekSchedules(string id)
        {
            var user = await _girafUserRepository.LoadUserWithWeekSchedules(id);

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
            if (usr == null) return null;
            return await _girafUserRepository.LoadBasicUserDataAsync(usr);
        }

        
        /// <summary>
        /// Checks if the user owns the given <paramref name="pictogram"/>.
        /// </summary>
        /// <param name="pictogram">The pictogram to check the ownership for.</param>
        /// <param name="user"></param>
        /// <returns>True if the user is authorized to see the resource and false if not.</returns>
        public async Task<bool> CheckPrivateOwnership(Pictogram pictogram, GirafUser user)
        {
            if (pictogram.AccessLevel != AccessLevel.PRIVATE)
                return false;

            //The pictogram was not public, check if the user owns it.
            if (user == null) return false;
            var ownedByUser = await _userResourseRepository.CheckPrivateOwnership(pictogram, user);

            return ownedByUser;
        }
        
        /// <summary>
        /// Creates a MD5 hash used for hashing pictures, and returns the hash as a string.
        /// </summary>
        /// <param name="image">Input image</param>
        /// <returns>The hash as a string</returns>
        public string GetHash(byte[] image)
        {
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(image);
                return Convert.ToBase64String(hash);
            }
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
            var ownedByDepartment = await _departmentResourseRepository.CheckProtectedOwnership(resource, user);

            return ownedByDepartment;
        }

    }
}
