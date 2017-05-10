using System;
using System.Collections.Generic;
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
        /// List of users the user is guardian of. Is simply null if the user isn't a guardian
        /// </summary>
        public List<GirafUserDTO> GuardianOf { get; set; }
        /// <summary>
        /// The Id of the user.
        /// </summary>
        public string Id { get; set; }
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
        public long DepartmentKey { get; set; }

        /// <summary>
        /// A list of the id's of the user's week schedules.
        /// </summary>
        public ICollection<long> WeekScheduleIds { get; set; }
        /// <summary>
        /// A list of the id's of the user's resources.
        /// </summary>
        public virtual ICollection<long> Resources { get; set; }

        /// <summary>
        /// A field for storing all the relevant GirafLauncher options.
        /// </summary>
        public LauncherOptions LauncherOptions { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public GirafUserDTO()
        {
            WeekScheduleIds = new List<long>();
            Resources = new List<long>();
            LauncherOptions = new LauncherOptions();
        }

        /// <summary>
        /// Creates a new data transfer object from a given user.
        /// </summary>
        /// <param name="user">The user to create a DTO for.</param>
        public GirafUserDTO(GirafUser user) 
        {
            Id = user.Id;
            Username = user.UserName;
            DisplayName = user.DisplayName;
            UserIcon = user.UserIcon;
            if (user.GuardianOf != null){
                GuardianOf = new List<GirafUserDTO>();
                foreach(var usr in user.GuardianOf)
                    GuardianOf.Add(new GirafUserDTO(usr));
            }
            if (user.Department == null)
                DepartmentKey = -1;
            else 
                DepartmentKey = user.DepartmentKey;
            WeekScheduleIds = user.WeekSchedule.Select(w => w.Id).ToList();
            Resources = new List<long>();
            foreach (var res in user.Resources)
            {
                Resources.Add(res.Key);
            }
            LauncherOptions = user.LauncherOptions;
        }
    }
}
