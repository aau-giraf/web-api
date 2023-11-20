using System;
using GirafRest.Models.Enums;

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
        /// The users profile picture.
        /// </summary>
        public virtual byte[] UserIcon { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DisplayNameDTO(string displayName, GirafRoles userRole, string userId, byte[] userIcon = null)
        {
            DisplayName = displayName;
            UserRole = userRole.ToString();
            UserId = userId;
            UserIcon = userIcon;
        }

        /// <summary>
        /// Empty constructor for JSON
        /// </summary>
        public DisplayNameDTO()
        {
        }

        public int CompareTo(DisplayNameDTO other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return this.DisplayName.CompareTo(other.DisplayName);
            }

        }

        public override bool Equals(object obj)
        {

            if (obj == null)
                return false;
            if (obj.GetType() != typeof(DisplayNameDTO))
                return false;

            DisplayNameDTO dto = (DisplayNameDTO)obj;
            return this.DisplayName == dto.DisplayName && this.UserRole == dto.UserRole && this.UserId == dto.UserId;
        }
    }
}
