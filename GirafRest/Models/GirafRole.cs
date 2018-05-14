using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    /// <summary>
    /// All the roles available in the system
    /// </summary>
    public class GirafRole : IdentityRole<string>
    {
        // Roles
        public const string Citizen = "Citizen";
        public const string Guardian = "Guardian";
        public const string Department = "Department";
        public const string SuperUser = "SuperUser";

        // DO NOT DELETE
        public GirafRole()
        {

        }

        public GirafRole(string name)
        {
            Name = name;
        }
    }
}
