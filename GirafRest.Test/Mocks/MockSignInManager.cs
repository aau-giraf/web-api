using GirafRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GirafRest.Test.Mocks
{
    class MockSignInManager : SignInManager<GirafUser>
    {
        public MockSignInManager(MockUserManager mockUserManager, IHttpContextAccessor contextAccessor)
            : base(mockUserManager, contextAccessor, null, null, null)
        {

        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return Task.FromResult(SignInResult.Success);
        }

        public override Task SignInAsync(GirafUser user, bool isPersistent, string authenticationMethod = null)
        {
            return Task.FromResult(0);
        }

        public override Task SignOutAsync()
        {
            return Task.FromResult(0);
        }
    }
}
