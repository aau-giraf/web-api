namespace GirafRest.Models.DTOs
{
    public class UserNameDTO
    {
        public string UserName { get; set; }
        public GirafRoles UserRole { get; private set; }
        public string UserId { get; set; }


        public UserNameDTO(string username, GirafRoles userRole, string userId)
        {
            UserName = username;
            UserRole = userRole;
            UserId = userId;
        }

        public UserNameDTO()
        {
        }
    }
}
