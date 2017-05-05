using GirafRest.Services;
using System;
using System.Collections.Generic;
using System.Text;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace GirafRest.Test.Mocks
{
    class MockGirafService : IGirafService
    {
        public ILogger _logger { get; set; }

        public GirafDbContext _context
        {
            get;
            private set;
        }

        public UserManager<GirafUser> _userManager
        {
            get;
            private set;
        }

        public MockGirafService(MockDbContext context, MockUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Task<bool> CheckPrivateOwnership(Pictogram resource, GirafUser user)
        {
            if (user == null)
                return Task.FromResult(false);

            var ownsResource = _context.UserResources
                .Where(ur => ur.Resource == resource && ur.Other == user)
                .Any();

            if (ownsResource)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }

        public Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user)
        {
            if (user == null)
                return Task.FromResult(false);

            var ownsResource = _context.DepartmentResources
                .Where(dr => dr.ResourceKey == resource.Id && dr.OtherKey == user.DepartmentKey)
                .Any();
            if (ownsResource)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }

        public Task<GirafUser> LoadUserAsync(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }

        public Task<byte[]> ReadRequestImage(Stream bodyStream)
        {
            byte[] image = new byte[bodyStream.Length];
            bodyStream.Read(image, 0, image.Length);

            return Task.FromResult(image);
        }

        public Task<GirafUser> LoadByNameAsync(string username)
        {
            return _userManager.FindByNameAsync(username);
        }
    }
}
