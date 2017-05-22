using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    /// <summary>
    /// All the roles and policies available in the system. 
    /// </summary>
    public class GirafRole : IdentityRole
    {
        // Roles
        public const string Citizen = "Citizen";
        public const string Guardian = "Guardian";
        public const string Department = "Department";
        public const string SuperUser = "SuperUser";

        // Policies
        public const string RequireCitizen = "RequireCitizen";
        public const string RequireSuperUser = "RequireSuperUser";
        public const string RequireGuardian = "RequireGuardian";
        public const string RequireGuardianOrSuperUser = "RequireGuardianOrSuperUser";
        public const string RequireDepartment = "RequireDepartment";

        public GirafRole()
        {

        }

        public GirafRole(string name)
        {
            Name = name;
        }
    }
}
