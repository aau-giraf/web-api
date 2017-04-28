using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
    /// <summary>
    /// This class is used when a new user is to be created. It simply defines the structure of the expected
    /// json string.
    /// </summary>
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public long DepartmentId { get; set; }
    }
}
