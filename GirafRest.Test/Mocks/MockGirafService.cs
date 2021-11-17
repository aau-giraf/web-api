using GirafRest.Services;
using System;
using System.Collections.Generic;
using System.Text;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using GirafRest.IRepositories;
using GirafRest.Test.RepositoryMocks;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Test.Mocks
{
    public class MockGirafService : IGirafService
    {
        private readonly IGirafUserRepository _girafUserRepository;
        private readonly IUserResourseRepository _userResourseRepository;
        private readonly IDepartmentResourseRepository _departmentResourseRepository;
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
        public MockGirafService( MockedUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<bool> CheckPrivateOwnership(Pictogram pictogram, GirafUser user)
        {
            if (user == null)
                return Task.FromResult(false);

            var ownsResource = _userResourseRepository.CheckIfUserOwnsResource(pictogram, user);

            if (ownsResource)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }

        public Task<bool> CheckProtectedOwnership(Pictogram pictogram, GirafUser user)
        {
            if (user == null)
                return Task.FromResult(false);

            var ownsResource = _departmentResourseRepository.CheckIfUserOwnsResource(pictogram, user);
            
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
            return Task.FromResult(_girafUserRepository.GetUserWithId(id));
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
