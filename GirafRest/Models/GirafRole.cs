using Microsoft.AspNetCore.Identity;

namespace GirafRest.Models
{
    /// <summary>
    /// All the roles available in the system
    /// </summary>
    public class GirafRole : IdentityRole
    {
        #region Roles
        /// <summary>
        /// Constant for Citizen
        /// </summary>
        public const string Citizen = "Citizen";
        /// <summary>
        /// Constant for Guardian
        /// </summary>
        public const string Guardian = "Guardian";
        /// <summary>
        /// Constant for Department
        /// </summary>
        public const string Department = "Department";
        /// <summary>
        /// Constant for SuperUser
        /// </summary>
        public const string SuperUser = "SuperUser";
        #endregion

        /// <summary>
        /// Empty constructor used for JSON Generation
        /// </summary>
        public GirafRole()
        {
        }

        /// <summary>
        /// Initialize with name
        /// </summary>
        public GirafRole(string name)
        {
            Name = name;
        }
    }
}
