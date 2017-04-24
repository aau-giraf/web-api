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

namespace GirafRest.Test.Mocks
{
    public class MockUserManager : UserManager<GirafUser>
    {
        GirafUser currentUser;

        public MockUserManager(IUserStore<GirafUser> store)
            : base(store, null, null, null, null, null, null, null, null)
        {
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
    }
}
