namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for Username
    /// </summary>
    public class UserNameDTO
    {
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Role for user
        /// </summary>
        public string UserRole { get; private set; }

        /// <summary>
        /// ID For user
        /// </summary>
        public string UserId { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public UserNameDTO(string username, GirafRoles userRole, string userId)
        {
            UserName = username;
            UserRole = userRole.ToString();
            UserId = userId;
        }

        /// <summary>
        /// Empty constructor for JSON
        /// </summary>
        public UserNameDTO()
        {
        }
    }
}
