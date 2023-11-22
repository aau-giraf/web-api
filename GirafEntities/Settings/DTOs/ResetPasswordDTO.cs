using System.ComponentModel.DataAnnotations;

namespace GirafEntities.Settings.DTOs
{
    /// <summary>
    /// This class defines the structure of the expected json when a user wishes to reset his password.
    /// </summary>
    public class ResetPasswordDTO
    {
        /// <summary>
        /// The users password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Reset password token. Used when a user request a password reset.
        /// </summary>
        public string Token { get; set; }
    }
}
