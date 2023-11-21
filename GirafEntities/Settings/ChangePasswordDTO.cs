using System.ComponentModel.DataAnnotations;

namespace GirafEntities.Settings
{
    /// <summary>
    /// DTO Used for changing password
    /// </summary>
    public class ChangePasswordDTO
    {
        /// <summary>
        /// The users current password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        /// <summary>
        /// The desired password.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
    }
}
