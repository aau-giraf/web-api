namespace GirafRest.Models.DTOs
{
    public class UserNameDTO
    {
        public string UserName { get; set; }
        public string UserId { get; set; }


        public UserNameDTO(string username, string userId)
        {
            UserName = username;
            UserId = userId;
        }

        public UserNameDTO()
        {
        }
    }
}
