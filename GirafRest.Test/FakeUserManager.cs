using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using GirafRest.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GirafRest.Test
{
    public class FakeUserManager : UserManager<GirafUser>
    {
        GirafUser currentUser;

        public FakeUserManager(IUserStore<GirafUser> store)
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
