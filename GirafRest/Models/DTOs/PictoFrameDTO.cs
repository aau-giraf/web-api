namespace GirafRest.Models.DTOs
{
    public class PictoFrameDTO : FrameDTO
    {
        public string Title { get; set; }
        public string Owner_Id { get; set; }
        public long Department_Key { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
}