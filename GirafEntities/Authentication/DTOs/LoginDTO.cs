﻿using System.ComponentModel.DataAnnotations;

namespace GirafEntities.Authentication.DTOs
{
    /// <summary>
    /// DTO Used for Login
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
