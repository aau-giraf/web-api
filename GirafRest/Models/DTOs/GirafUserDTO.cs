using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Giraf roles
    /// </summary>
    public enum GirafRoles { Citizen, Department, Guardian, SuperUser }

    /// <summary>
    /// Defines the structure of GirafUsers when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class GirafUserDTO
    {
        /// <summary>
        /// List of the roles the current user is defined as in the system.
        /// </summary>
        public GirafRoles Role { get; set; }

        /// <summary>
        /// List of the roles the current user is defined as in the system, as strings
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// The Id of the user.
        /// </summary>c
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// The username of the user.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The display name of the user.
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// The key of the user's department.
        /// </summary>
        public long? Department { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public GirafUserDTO()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GirafRest.Models.DTOs.GirafUserDTO"/> class.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="userRole">User role.</param>
        /// <param name="addGuardianRelation">If set to <c>true</c> add guardian relation.</param>
        public GirafUserDTO(GirafUser user, GirafRoles userRole)
        {
            //Add all trivial values
            Id = user.Id;
            Username = user.UserName;
            ScreenName = user.DisplayName;
            RoleName = userRole.ToString();
            Role = userRole;

            //Check if a user is in a department, add null as key if not.
            if (user.Department == null && user.DepartmentKey == -1)
                Department = null;
            else
                Department = user.DepartmentKey;
        }
    }
}
