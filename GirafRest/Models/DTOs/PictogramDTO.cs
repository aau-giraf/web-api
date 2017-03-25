namespace GirafRest.Models.DTOs
{
    public class PictogramDTO : PictoFrameDTO
    {
        public byte[] Image { get; set; }
        public PictogramDTO(Pictogram Pictogram)
        {
            this.AccessLevel = Pictogram.AccessLevel;
            this.Id = Pictogram.Key;
            this.Title = Pictogram.Title;
            this.LastEdit = Pictogram.LastEdit;
        }

        public PictogramDTO(Pictogram pictogram, byte[] image) : this(pictogram)
        {
            this.Image = image;
        }
    }
}