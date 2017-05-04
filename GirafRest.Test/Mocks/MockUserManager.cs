using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using GirafRest.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static GirafRest.Test.UnitTestExtensions;
using System.Linq;

namespace GirafRest.Test.Mocks
{
    public class MockUserManager : UserManager<GirafUser>
    {
        GirafUser currentUser;
        private readonly TestContext _testContext;

        public MockUserManager(IUserStore<GirafUser> store, TestContext testContext)
            : base(store, null, null, null, null, null, null, null, null)
        {
            _testContext = testContext;
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

        public override Task<bool> IsInRoleAsync(GirafUser user, string role)
        {
            return Task.FromResult(_testContext.MockUserRoles.Where(ur => ur.RoleId == role && ur.UserId == user.Id).Any());
        }

        public override Task<GirafUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(_testContext.MockUsers.Where(u => u.UserName == userName).FirstOrDefault());
        }
    }
}
