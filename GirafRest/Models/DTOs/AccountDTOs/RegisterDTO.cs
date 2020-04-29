using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
    /// <summary>
    /// DTO Used for registrering/Signup
    /// </summary>
    public class RegisterDTO
    {
        /// <summary>
        /// The users username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The users password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// The users DisplayName
        /// </summary>
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// The users departmentid.
        /// </summary>
        [Required]
        public long? DepartmentId { get; set; }

        /// <summary>
        /// Signup role
        /// </summary>
        [Required]
        public GirafRoles Role { get; set; }
    }
}
