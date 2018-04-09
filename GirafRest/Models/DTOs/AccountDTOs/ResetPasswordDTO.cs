using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
    /// <summary>
    /// This class defines the structure of the expected json when a user wishes to reset his password.
    /// </summary>
    public class ResetPasswordDTO
    {
        /// <summary>
        /// The users username.
        /// </summary>
        [Required(ErrorMessage = "Indtast venligst dit brugernavn her.")]
        [Display(Name = "Brugernavn")]
        public string Username { get; set; }

        /// <summary>
        /// The users password.
        /// </summary>
        [Required(ErrorMessage = "Indtast venligst dit kodeord her.")]
        [DataType(DataType.Password)]
        [Display(Name = "Kodeord")]
        public string Password { get; set; }

        /// <summary>
        /// Reset password confirmation code. Used when a user request a password reset, this code needs to be added to the url in order to reset.
        /// </summary>
        public string Code { get; set; }
    }
}
