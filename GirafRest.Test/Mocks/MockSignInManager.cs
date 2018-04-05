using GirafRest.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test.Mocks
{
    class MockSignInManager : SignInManager<GirafUser>
    {
        public List<Tuple<string, string>> UserNamePasswordList;

        public MockSignInManager(MockUserManager mum, TestContext tc)
            : base(mum, 
                  //The following mocks must be there, as SignInManager throws an exception if they are null.
                  new Mock<IHttpContextAccessor>().Object, 
                  new Mock<IUserClaimsPrincipalFactory<GirafUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<ILogger<SignInManager<GirafUser>>>().Object,
                  new Mock<IAuthenticationSchemeProvider>().Object)
        {
            mum._signInManager = this;
            UserNamePasswordList = tc.MockUsers.Select(u => new Tuple<string, string>(u.UserName, "password")).ToList();
        }

        public override Task<SignInResult> PasswordSignInAsync(string UserName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            if (UserNamePasswordList.Where(up => up.Item1 == UserName && up.Item2 == password).Any())
                return Task.FromResult(SignInResult.Success);
            else
                return Task.FromResult(SignInResult.Failed);
        }

        public override Task SignInAsync(GirafUser user, bool isPersistent, string authenticationMethod = null)
        {
            return Task.FromResult(SignInResult.Success);
        }

        public override Task SignOutAsync()
        {
            return Task.FromResult(0);
        }
    }
}
