﻿using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Moq;
namespace GirafRest.Test.Mocks
{
    public class MockUserManagerDepartment : UserManager<GirafUser>
    {
        public GirafUser currentUser;
        public MockUserManagerDepartment() : base(new Mock<IUserStore<GirafUser>>().Object, null, null, null, null, null, null, null, null)
        {

        }


        public void MockLoginAsUser(GirafUser user)
        {
            currentUser = user;
        }
        public override Task<GirafUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return Task.FromResult(currentUser);
        }
        public override Task<bool> IsInRoleAsync(GirafUser user, string role)
        {
            return Task.FromResult(false);
        }
       
    }



}
