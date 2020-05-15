namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for DisplayName, used to avoid sending entire GirafUserDTO to avoid sending unnecessary information
    /// </summary>
    public class DisplayNameDTO
    {
        /// <summary>
        /// DisplayName
        /// </summary>
        public string DisplayName { get; set; }

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
        public DisplayNameDTO(string displayName, Role userRole, string userId)
        {
            DisplayName = displayName;
            UserRole = userRole.ToString();
            UserId = userId;
        }

        /// <summary>
        /// Empty constructor for JSON
        /// </summary>
        public DisplayNameDTO()
        {
        }
    }
}
