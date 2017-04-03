namespace GirafRest.Models.DTOs
{
    public class PictoFrameDTO : FrameDTO
    {
        public string Title { get; set; }
        public AccessLevel AccessLevel { get; set; }

        public PictoFrameDTO(PictoFrame pictoframe) : base(pictoframe)
        {
            this.Title = pictoframe.Title;
            this.AccessLevel = pictoframe.AccessLevel;
        }
    }
}