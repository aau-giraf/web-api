using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    public class GirafRole : IdentityRole
    {
        // Roles
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Parent = "Parent";
        public const string Guardian = "Guardian";

        // Policies
        public const string RequireUser = "RequireUser";
        public const string RequireAdmin = "RequireAdmin";
        public const string RequireParent = "RequireParent";
        public const string RequireGuardian = "RequireGuardian";
        public const string RequireGuardianOrAdmin = "RequireGuardianOrAdmin";

        public GirafRole()
        {

        }

        public GirafRole(string name)
        {
            Name = name;
        }
    }
}
