using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public UserRole()
        {
        }

        public UserRole(GirafUser girafUser, GirafRole girafRole)
        {
            girafUser.Roles.Add(this);
            girafRole.Users.Add(this);
        }
    }
}
