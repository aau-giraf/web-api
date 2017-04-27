using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafRest.Models
{
    /// <summary>
    /// GirafUser defines all relavant data for the user's of Giraf.
    /// </summary>
    [Table("User")]
    public class GirafUser : IdentityUser
    {        
        /// <summary>
        /// The display name for the user.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// The profile icon of the user.
        /// </summary>
        public byte[] UserIcon { get; set; }

        /// <summary>
        /// The key of the user's department.
        /// </summary>
        public long DepartmentKey { get; set; }
        /// <summary>
        /// A reference to the user's department.
        /// </summary>
        [ForeignKey("DepartmentKey")]
        public virtual Department Department { get; set; }

        /// <summary>
        /// A collection of the user's week schedules.
        /// </summary>
        public ICollection<Week> WeekSchedule { get; set; }
        /// <summary>
        /// A collection of the user's resources.
        /// </summary>
        public virtual ICollection<UserResource> Resources { get; set; }

        /// <summary>
        /// A flag indicating whether to run applications in grayscale or not.
        /// </summary>
        public bool UseGrayscale { get; set; }
        /// <summary>
        /// A flag indicating whether to display animations in the launcher or not.
        /// </summary>
        public bool DisplayLauncherAnimations { get; set; }
        /// <summary>
        /// A collection of all the user's applications.
        /// </summary>
        public ICollection<ApplicationOption> AvailableApplications { get; set; }
        
        /// <summary>
        /// Creates a new user with the specified user name, associated with the given department.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="departmentId">The id of the department to which the user should be added.</param>
        public GirafUser (string userName, long departmentId) : base(userName)
        {
            this.UserName = userName;
            this.Resources = new List<UserResource>();
            this.DepartmentKey = departmentId;
            this.WeekSchedule = new List<Week>();
            this.WeekSchedule.Add(new Week());
            this.WeekSchedule.First().InitWeek();
            AvailableApplications = new List<ApplicationOption>();
        }
        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public GirafUser()
        {
            this.Resources = new List<UserResource>();
            this.WeekSchedule = new List<Week>();
            this.WeekSchedule.Add(new Week());
            this.WeekSchedule.First().InitWeek();
            AvailableApplications = new List<ApplicationOption>();
        }
    }
}