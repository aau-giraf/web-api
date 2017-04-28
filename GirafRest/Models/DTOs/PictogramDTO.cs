namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of Pictogram when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class PictogramDTO : PictoFrameDTO
    {
        /// <summary>
        /// An array of bytes containing the pictogram's image.
        /// </summary>
        public byte[] Image { get; set; }
        /// <summary>
        /// Defines the file type of the pictogram's image.
        /// </summary>
        public PictogramImageFormat ImageFormat { get; set; }

        /// <summary>
        /// Creates a new pictogram data transfer object from a given pictogram.
        /// </summary>
        /// <param name="Pictogram">The pictogram to create a DTO for.</param>
        public PictogramDTO(Pictogram Pictogram) : base(Pictogram)
        {
        }

        /// <summary>
        /// Creates a new pictogram data transfer object from a given pictogram, which also includes the pictogram's image.
        /// </summary>
        /// <param name="pictogram">The pictogram to create a DTO for.</param>
        /// <param name="image">An array of bytes containing the pictogram's image.</param>
        public PictogramDTO(Pictogram pictogram, byte[] image) : this(pictogram)
        {
            this.Image = image;
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public PictogramDTO() {}
    }
}