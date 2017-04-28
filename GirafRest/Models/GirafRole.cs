using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    public class GirafRole : IdentityRole
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Parent = "Parent";
        public const string Guardian = "Guardian";
        public const string GuardianOrAdmin = "Guadian,Admin";
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IPAddress { get; set; }

        public GirafRole()
        {

        }

        public GirafRole(string name)
        {
            Name = name;
        }
    }
}
