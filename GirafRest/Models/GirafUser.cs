using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafRest.Models
{
    /// <summary>
    /// GirafUser defines all relavant data for the user's of Giraf.
    /// </summary>
    [Table("User")]
    public class GirafUser : IdentityUser<string>
    {       
        /// <summary>
        /// Whether or not the current user is a DepartmentUser
        /// </summary>
        public bool IsDepartment { get; set; }

        /// <summary>
        /// List of users the user is guardian of. Is simply null if the user isn't a guardian
        /// </summary>
        public List<GirafUser> GuardianOf { get; set; }

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
        [ForeignKey("Department")]
        public long DepartmentKey { get; set; }
        /// <summary>
        /// A reference to the user's department.
        /// </summary>
        public virtual Department Department { get; set; }

        /// <summary>
        /// A collection of the user's week schedules.
        /// </summary>
        public virtual ICollection<Week> WeekSchedule { get; set; }

        /// <summary>
        /// A collection of the user's resources.
        /// </summary>
        public virtual ICollection<UserResource> Resources { get; set; }

        /// <summary>
        /// A field for storing all relevant options that the user has specified in the GirafLauncher.
        /// </summary>
        public LauncherOptions Settings { get; set; }
        
        /// <summary>
        /// Creates a new user with the specified user name, associated with the given department.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="department">The department to which the user should be added.</param>
        public GirafUser (string userName, Department department) : base(userName)
        {
            this.UserName = userName;
            this.Resources = new List<UserResource>();
            this.GuardianOf = new List<GirafUser>();
            this.WeekSchedule = new List<Week>();
            Settings = new LauncherOptions();

            DepartmentKey = department?.Key ?? -1;
        }
        
        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public GirafUser()
        {
            this.Resources = new List<UserResource>();
            this.WeekSchedule = new List<Week>();
            Settings = new LauncherOptions();
        }
    }
}