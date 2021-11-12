using System;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for DisplayName, used to avoid sending entire GirafUserDTO to avoid sending unnecessary information
    /// </summary>
    public class DisplayNameDTO : IComparable<DisplayNameDTO>
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
        public DisplayNameDTO(string displayName, GirafRoles userRole, string userId)
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

        public int CompareTo(DisplayNameDTO other)
        {
           if(other == null)
            {
                return 1;
            }
            else
            {
                return this.DisplayName.CompareTo(other.DisplayName);
            }

        }
    }
}
