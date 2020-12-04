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
using Microsoft.EntityFrameworkCore;

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
        public string GetHash(byte[] image)
        {
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(image);
                return Convert.ToBase64String(hash);
            }
        }
        public MockGirafService(GirafDbContext context, MockUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Task<bool> CheckPrivateOwnership(Pictogram pictogram, GirafUser user)
        {
            if (user == null)
                return Task.FromResult(false);

            var ownsResource = _context.UserResources
                .Any(ur => ur.Pictogram == pictogram && ur.Other == user);

            if (ownsResource)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }

        public Task<bool> CheckProtectedOwnership(Pictogram pictogram, GirafUser user)
        {
            if (user == null)
                return Task.FromResult(false);

            var ownsResource = _context.DepartmentResources
                .Any(dr => dr.PictogramKey == pictogram.Id 
                            && dr.OtherKey == user.DepartmentKey);
            
            if (ownsResource)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }

        public Task<GirafUser> LoadBasicUserDataAsync(ClaimsPrincipal principal)
        {
            //Same as above, because it is not the job of unit-tests to simulate that.
            return _userManager.GetUserAsync(principal);
        }

        public Task<byte[]> ReadRequestImage(Stream bodyStream)
        {
            byte[] image = new byte[bodyStream.Length];
            bodyStream.Read(image, 0, image.Length);

            return Task.FromResult(image);
        }

        public Task<GirafUser> LoadUserWithWeekSchedules(string id)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id == id));
        }

        public Task<GirafUser> LoadUserWithResources(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }

        public Task<GirafUser> LoadUserWithDepartment(ClaimsPrincipal principal)
        {
            return _userManager.GetUserAsync(principal);
        }
    }
}
