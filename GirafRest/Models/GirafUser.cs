using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafRest.Models
{
    [Table("User")]
    public class GirafUser : IdentityUser
    {        
        public long DepartmentKey { get; set; }
        [ForeignKey("DepartmentKey")]
        public virtual Department Department { get; set; }

        public ICollection<Week> WeekSchedule { get; set; }
        
        public virtual ICollection<UserResource> Resources { get; set; }

        public byte[] UserIcon {get; set;}
    
        public GirafUser (string userName, long departmentId) : base(userName)
        {
            this.UserName = userName;
            this.Resources = new List<UserResource>();
            this.DepartmentKey = departmentId;
            this.WeekSchedule = new List<Week>();
            this.WeekSchedule.Add(new Week());
            this.WeekSchedule.First().InitWeek();
        }
        public GirafUser()
        {
            this.Resources = new List<UserResource>();
            this.WeekSchedule = new List<Week>();
            this.WeekSchedule.Add(new Week());
            this.WeekSchedule.First().InitWeek();
        }
    }
}