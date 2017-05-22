using System.Collections.Generic;
using GirafRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GirafRest.Test.Mocks
{
    public class MockRoleManager : RoleManager<GirafRole>
    {
        public MockRoleManager(IRoleStore<GirafRole> store)
            : base(store, null, null, null, null, null)
        {
        }
    }
}