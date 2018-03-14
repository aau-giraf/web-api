using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of GirafUsers when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class GirafUserDTO
    {
        /// <summary>
        /// Enum over roles.
        /// </summary>
        public enum GirafRoles { Citizen, Department, Guardian, SuperUser }
        /// <summary>
        /// List of the roles the current user is defined as in the system.
        /// </summary>
        public GirafRoles Role { get; set; }
        /// <summary>
        /// List of users the user is guardian of. Is simply null if the user isn't a guardian. Contains guardians if the user is a Department
        /// </summary>
        public List<GirafUserDTO> Citizens { get; set; }
        /// <summary>
        /// Gets or sets guardians of a user.
        /// </summary>
        /// <value>My guardians.</value>
        public List<GirafUserDTO> Guardians { get; set; }
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
        public string ScreenName { get; set; }
        /// <summary>
        /// A byte array containing the user's profile icon.
        /// </summary>
        public byte[] UserIcon { get; set; }

        /// <summary>
        /// The key of the user's department.
        /// </summary>
        public long? Department { get; set; }
        [Required]
        /// <summary>
        /// A list of the id's of the user's week schedules.
        /// </summary>
        public ICollection<WeekDTO> WeekScheduleIds { get; set; }
        [Required]
        /// <summary>
        /// A list of the id's of the user's resources.
        /// </summary>
        public virtual ICollection<ResourceDTO> Resources { get; set; }
        [Required]
        /// <summary>
        /// A field for storing all the relevant GirafLauncher options.
        /// </summary>
        public LauncherOptionsDTO Settings { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public GirafUserDTO()
        {
            WeekScheduleIds = new List<WeekDTO>();
            Resources = new List<ResourceDTO>();
            Settings = new LauncherOptionsDTO();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GirafRest.Models.DTOs.GirafUserDTO"/> class.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="userRole">User role.</param>
        /// <param name="addGuardianRelation">If set to <c>true</c> add guardian relation.</param>
        public GirafUserDTO(GirafUser user, GirafRoles userRole, bool addGuardianRelation = true)
        {
            //Add all trivial values
            Id = user.Id;
            Username = user.UserName;
            ScreenName = user.DisplayName;
            UserIcon = user.UserIcon;
            Role = userRole;

            if (addGuardianRelation)
            {
                //Check if the user is guardian of any users and add DTOs for those if that is the case
                if (user.Citizens != null && user.Citizens.Any())
                {
                    Citizens = new List<GirafUserDTO>();
                    foreach (var usr in user.Citizens)
                    {
                        if(usr.Citizen != null){
                            Citizens.Add(new GirafUserDTO(usr.Citizen, GirafRoles.Citizen, false));
                        }
                    }
                }

                //Check if the user has guardians and add DTO's for those if that is the case
                if (user.Citizens != null && user.Guardians.Any())
                {
                    Guardians = new List<GirafUserDTO>();
                    foreach (var usr in user.Citizens){
                        if (usr.Guardian != null)
                        {
                            Guardians.Add(new GirafUserDTO(usr.Guardian, GirafRoles.Guardian, false));
                        }
                    }

                }
            }
            Console.WriteLine("Department = " + user.Department);
            //Check if a user is in a department, add null as key if not.
            if (user.Department == null)
                Department = null;
            else
                Department = user.DepartmentKey;

            //Add the ids of the user's weeks and resources
            WeekScheduleIds = user.WeekSchedule.Select(w => new WeekDTO(w)).ToList();
            var choices = user.Resources.Select(r => r.Resource).OfType<Choice>().Select(c => new ChoiceDTO(c)).AsEnumerable<ResourceDTO>();
            var pictograms = user.Resources.Select(r => r.Resource).OfType<Pictogram>().Select(c => new PictogramDTO(c)).AsEnumerable<ResourceDTO>();
            Resources = choices.Union(pictograms).ToList();

            //And finally the user's settings
            Settings = new LauncherOptionsDTO(user.Settings);
        }
    }
}
