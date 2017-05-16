using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
    /// <summary>
    /// This class is used when a new user is to be created. It simply defines the structure of the expected
    /// json string.
    /// </summary>
    public class RegisterDTO
    {
        /// <summary>
        /// The users username.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Brugernavn")]
        public string Username { get; set; }

        /// <summary>
        /// The users password.
        /// </summary>
        [Required]
        [Display(Name = "Kodeord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// The users password to avoid typos/mistakes.
        /// </summary>
        [Required]
        [Display(Name = "Bekræft kodeord")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "De indtastede kodeord passer ikke sammen.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// The users departmentid.
        /// </summary>
        [Required]
        [Display(Name = "Afdelingsnummer")]
        public long? DepartmentId { get; set; }
    }
}
