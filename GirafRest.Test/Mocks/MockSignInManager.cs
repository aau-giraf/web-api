using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GirafRest.Test.Mocks
{
    class MockSignInManager : SignInManager<GirafUser>
    {
        GirafUser currentUser;

        public MockSignInManager()
        {

        }
    }
}
