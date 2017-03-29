using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafRest.Models
{
    [Table("User")]
    public class GirafUser : IdentityUser
    {
        public static string[] GirafRoles = new string[] { "User", "Guardian", "Admin" };
        
        public long DepartmentKey { get; set; }
        [ForeignKey("DepartmentKey")]
        public virtual Department Department { get; set; }
        
        public virtual ICollection<UserResource> Resources { get; set; }
    
        public GirafUser (string userName, long departmentId) : base(userName)
        {
            this.UserName = userName;
            this.Resources = new List<UserResource>();
            this.DepartmentKey = departmentId;
        }
        public GirafUser()
        {
            this.Resources = new List<UserResource>();
        }
    }
}