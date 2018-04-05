namespace GirafRest.Models.DTOs
{
    public class UserNameDTO
    {
        public string UserName { get; set; }
        public string Id { get; set; }

        public static explicit operator UserNameDTO(GirafUserDTO guser) {
            return new UserNameDTO(guser.UserName, guser.Id);
        }

        public UserNameDTO(string UserName, string Id)
        {
            UserName = UserName;
            Id = Id;
        }

        public UserNameDTO()
        {
        }

    }
}
