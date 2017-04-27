using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.AccountViewModels
{
    /// <summary>
    /// This model is used when the user signs into the system. It defines the structure of the expected json-string.
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
