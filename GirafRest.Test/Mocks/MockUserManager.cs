using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using GirafRest.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using static GirafRest.Test.UnitTestExtensions;
using System.Linq;

namespace GirafRest.Test.Mocks
{
    public class MockUserManager : UserManager<GirafUser>
    {
        GirafUser currentUser;
        private readonly TestContext _testContext;
        internal MockSignInManager _signInManager;

        public MockUserManager(IUserStore<GirafUser> store, TestContext testContext)
            : base(store, null, null, null, null, null, null, null, null)
        {
            _testContext = testContext;
            _signInManager = new MockSignInManager(this, _testContext);
        }

        public void MockLoginAsUser(GirafUser user)
        {
            currentUser = user;
        }

        public void MockLogout()
        {
            currentUser = null;
        }

        public override Task<GirafUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return Task.FromResult(currentUser);
        }

        public override Task<IdentityResult> CreateAsync(GirafUser user)
        {
            if (_testContext.MockUsers.Any(u => u.UserName == user.UserName))
                return Task.FromResult(IdentityResult.Failed());

            _testContext.MockUsers.Add(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> CreateAsync(GirafUser user, string password)
        {
            var result = CreateAsync(user);
            if (result.Result.Succeeded)
            {
                _signInManager.usernamePasswordList.Add(new Tuple<string, string>(user.UserName, password));
            }
            return result;
        }

		public override Task<IList<string>> GetRolesAsync(GirafUser user)
		{
            var userRoles = _testContext.MockUserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId);
            var userRolesName = (Task.FromResult(_testContext.MockRoles.Where(ur => userRoles.Any(urid => urid == ur.Id))
                                                 .Select(r => r.Name).ToList() as IList<string>));
            return userRolesName;
		}

		public override Task<bool> IsInRoleAsync(GirafUser user, string role)
        {
            return Task.FromResult(_testContext.MockUserRoles.Where(ur => ur.RoleId == role && ur.UserId == user.Id).Any());
        }

        public override Task<GirafUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(_testContext.MockUsers.Where(u => u.UserName == userName).FirstOrDefault());
        }

        public override Task<GirafUser> FindByIdAsync(string userId)
        {
            return Task.FromResult(_testContext.MockUsers.Where(u => u.Id == userId).FirstOrDefault());
        }

        public override Task<string> GeneratePasswordResetTokenAsync(GirafUser user)
        {
            return Task.FromResult($"ResetTokenFor{user.UserName}");
        }

        public override Task<IdentityResult> SetUserNameAsync(GirafUser user, string userName)
        {
            user.UserName = userName;
            return Task.FromResult(new IdentityResult());
        }

        public override Task<IdentityResult> AddPasswordAsync(GirafUser user, string password)
        {
            _signInManager.usernamePasswordList.Add(new Tuple<string, string>(user.UserName, password));
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> ChangePasswordAsync(GirafUser user, string currentPassword, string newPassword)
        {
            var upIndex = _signInManager.usernamePasswordList
                .Where(up => up.Item1 == user.UserName && up.Item2 == currentPassword);
            if (upIndex.Any())
            {
                _signInManager.usernamePasswordList.Remove(upIndex.FirstOrDefault());
                _signInManager.usernamePasswordList.Add(new Tuple<string, string>(user.UserName, newPassword));
                return Task.FromResult(IdentityResult.Success);
            }
            else
                return Task.FromResult(IdentityResult.Failed());
        }

        public override Task<IdentityResult> AddToRoleAsync(GirafUser user, string role)
        {
            var mockRole = _testContext.MockRoles.Where(r => r.Id == role).FirstOrDefault();
            var mockUser = _testContext.MockUsers.Where(u => u.Id == user.Id).FirstOrDefault();
            _testContext.MockUserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = mockUser.Id,
                RoleId = mockRole.Id
            });
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
