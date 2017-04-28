using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
    /// <summary>
    /// This class defines the structure of the expected json when a user wishes to reset his password.
    /// </summary>
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
