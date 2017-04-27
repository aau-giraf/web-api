using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GirafRest.Services
{
    /// <summary>
    /// The IGirafService interfaces defines all methods that are commonly used by the controllers.
    /// </summary>
    public interface IGirafService
    {
        /// <summary>
        /// A reference to a logger used to log information from controllers.
        /// </summary>
        ILogger _logger
        {
            get;
            set;
        }
        /// <summary>
        /// A reference to the database context of the application. This context may be used to query for data.
        /// </summary>
        GirafDbContext _context
        {
            get;
        }
        /// <summary>
        /// A reference to the user manager, that is used to fetch users.
        /// </summary>
        UserManager<GirafUser> _userManager
        {
            get;
        }

        /// <summary>
        /// Loads the current user.
        /// </summary>
        /// <param name="principal">A reference to HttpContext.User</param>
        /// <returns>The loaded user.</returns>
        Task<GirafUser> LoadUserAsync(ClaimsPrincipal principal);
        /// <summary>
        /// Loads the image of the request body.
        /// </summary>
        /// <param name="bodyStream">A stream from which the request body may be read.</param>
        /// <returns>The image as a byte array.</returns>
        Task<byte[]> ReadRequestImage(Stream bodyStream);
        /// <summary>
        /// Checks if the current user owns the given resource.
        /// </summary>
        /// <param name="resource">The resource to check ownership for.</param>
        /// <param name="context">A reference to the HttpContext of the current request.</param>
        /// <returns>True if the user owns the resource, false if not.</returns>
        Task<bool> CheckPrivateOwnership(PictoFrame resource, HttpContext context);
        /// <summary>
        /// Checks if the current user's department owns the given resource.
        /// </summary>
        /// <param name="resource">The resource to check ownership for.</param>
        /// <param name="context">A reference to the HttpContext of the current request.</param>
        /// <returns>True if the user's department owns the resource, false if not.</returns>
        Task<bool> CheckProtectedOwnership(PictoFrame resource, HttpContext context);
    }
}
