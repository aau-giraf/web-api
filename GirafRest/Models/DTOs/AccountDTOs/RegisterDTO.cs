using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
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

        [Required]
        public GirafRoles Role { get; set; }
    }
}
