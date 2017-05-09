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
        [Display(Name = "Brugernavn")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Kodeord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Bekræft kodeord")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "De indtastede kodeord passer ikke sammen.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Afdelingsnummer")]
        public long? DepartmentId { get; set; }
    }
}
