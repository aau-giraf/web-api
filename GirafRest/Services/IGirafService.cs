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
    public interface IGirafService
    {
        ILogger _logger
        {
            get;
            set;
        }
        GirafDbContext _context
        {
            get;
        }
        UserManager<GirafUser> _userManager
        {
            get;
        }

        Task<GirafUser> LoadUserAsync(ClaimsPrincipal principal);
        Task<byte[]> ReadRequestImage(Stream bodyStream);
        Task<bool> CheckResourceOwnership(Frame resource, HttpContext context);

    }
}
