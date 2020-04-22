namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for ScreemName, used to avoid sending entire GirafUserDTO to avoid sending unnecessary information
    /// </summary>
    public class ScreenNameDTO
    {
        /// <summary>
        /// ScreenName
        /// </summary>
        public string ScreenName { get; set; }

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
        public ScreenNameDTO(string screenName, GirafRoles userRole, string userId)
        {
            ScreenName = screenName;
            UserRole = userRole.ToString();
            UserId = userId;
        }

        /// <summary>
        /// Empty constructor for JSON
        /// </summary>
        public ScreenNameDTO()
        {
        }
    }
}
