namespace GirafRest.Models.DTOs
{
    public class PictogramDTO : PictoFrameDTO
    {
        public byte[] Image { get; set; }
        public PictogramDTO(Pictogram Pictogram) : base(Pictogram)
        {
        }

        public PictogramDTO(Pictogram pictogram, byte[] image) : this(pictogram)
        {
            this.Image = image;
        }
    }
}