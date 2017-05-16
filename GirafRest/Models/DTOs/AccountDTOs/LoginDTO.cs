using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs.AccountDTOs
{
    /// <summary>
    /// This model is used when the user signs into the system. It defines the structure of the expected json-string.
    /// </summary>
    public class LoginDTO
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
    }
}
