using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of GiradUsers when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class GirafUserDTO
    {
        /// <summary>
        /// Enum over roles.
        /// </summary>
        public enum GirafRoles { Admin, Citizen, Department, Guardian }
        /// <summary>
        /// List of the roles the current user is defined as in the system.
        /// </summary>
        public GirafRoles Role { get; set; }
        [Required]
        /// <summary>
        /// List of users the user is guardian of. Is simply null if the user isn't a guardian
        /// </summary>
        public List<GirafUserDTO> GuardianOf { get; set; }
        [Required]
        /// <summary>
        /// The Id of the user.
        /// </summary>
        public string Id { get; set; }
        [Required]
        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The display name of the user.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// A byte array containing the user's profile icon.
        /// </summary>
        public byte[] UserIcon { get; set; }

        /// <summary>
        /// The key of the user's department.
        /// </summary>
        public long? DepartmentKey { get; set; }
        [Required]
        /// <summary>
        /// A list of the id's of the user's week schedules.
        /// </summary>
        public ICollection<WeekDTO> WeekScheduleIds { get; set; }
        [Required]
        /// <summary>
        /// A list of the id's of the user's resources.
        /// </summary>
        public virtual ICollection<long> Resources { get; set; }
        [Required]
        /// <summary>
        /// A field for storing all the relevant GirafLauncher options.
        /// </summary>
        public LauncherOptions Settings { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public GirafUserDTO()
        {
            WeekScheduleIds = new List<WeekDTO>();
            Resources = new List<long>();
            Settings = new LauncherOptions();
        }

        /// <summary>
        /// Creates a new data transfer object from a given user.
        /// </summary>
        /// <param name="user">The user to create a DTO for.</param>
        public GirafUserDTO(GirafUser user, GirafRoles userRole) 
        {
            //Add all trivial values
            Id = user.Id;
            Username = user.UserName;
            DisplayName = user.DisplayName;
            UserIcon = user.UserIcon;
            Role = userRole;

            //Check if the user is guardian of any users and add DTOs for those if that is the case
            if (user.GuardianOf != null){
                GuardianOf = new List<GirafUserDTO>();
                foreach(var usr in user.GuardianOf)
                    GuardianOf.Add(new GirafUserDTO(usr, GirafRoles.Citizen));
            }

            Console.WriteLine("Department = " + user.Department);
            //Check if a user is in a department, add null as key if not.
            if (user.Department == null)
                DepartmentKey = null;
            else 
                DepartmentKey = user.DepartmentKey;

            //Add the ids of the user's weeks and resources
            WeekScheduleIds = user.WeekSchedule.Select(w => new WeekDTO(w)).ToList();
            Resources = user.Resources.Select(r => r.ResourceKey).ToList();
            
            //And finally the user's settings
            Settings = user.Settings;
        }
    }
}
