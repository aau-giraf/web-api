using System.Collections.Generic;
using GirafRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
namespace GirafRest.Test.Mocks
{
    public class MockRoleManager : RoleManager<GirafRole>
    {

        public MockRoleManager()
           : this(new Mock<IRoleStore<GirafRole>>())
        { }
        public MockRoleManager(Mock<IRoleStore<GirafRole>> store)
            : base(store.Object, null, null, null, null)
        {
        }


    }
}