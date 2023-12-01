using System.Collections.Generic;
using System.Threading.Tasks;
using GirafEntities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
namespace Giraf.UnitTest.Mocks
{
    public class MockRoleManager : RoleManager<GirafRole>
    {

        private readonly List<GirafRoles> rolesList;
        

        public MockRoleManager(List<GirafRoles> roles)
           : this(new Mock<IRoleStore<GirafRole>>())
        {
            rolesList = roles;
        }
        public MockRoleManager(Mock<IRoleStore<GirafRole>> store)
            : base(store.Object, null, null, null, null)
        {
        }

        public Task<GirafRoles> findUserRole(UserManager<GirafUser> userManager,GirafUser user)
        {
            var ele = rolesList[0];
            this.rolesList.RemoveAt(0);
            return Task.FromResult<GirafRoles>(ele);
        }


    }
}