using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafRest.Models
{
    [Table("User")]
    public class GirafUser : IdentityUser
    {
        public static string[] GirafRoles = new string[] { "User", "Guardian", "Admin" };
        
        public long WeekKey { get; set; }

        [ForeignKey("WeekKey")]
        public Week WeekSchedule { get; set; }
        public long DepartmentKey { get; set; }
        [ForeignKey("DepartmentKey")]
        public virtual Department Department { get; set; }
        
        public virtual ICollection<UserResource> Resources { get; set; }
    
        public GirafUser (string userName) : base(userName)
        {
            this.UserName = userName;
            this.Resources = new List<UserResource>();
        }
        public GirafUser()
        {
            this.Resources = new List<UserResource>();
        }
    }
}