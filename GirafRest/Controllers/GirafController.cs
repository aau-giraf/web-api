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
    }
}
