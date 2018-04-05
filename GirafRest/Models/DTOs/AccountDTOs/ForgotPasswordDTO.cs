using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
    /// <summary>
    /// The ForgotPasswordViewModel is used when a user has forgot his password and request a new one. It
    /// simply defines the structure of the json-string that must be sent to request a new password.
    /// </summary>
    public class ForgotPasswordDTO
    {
        /// <summary>
        /// The users UserName.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// His email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
