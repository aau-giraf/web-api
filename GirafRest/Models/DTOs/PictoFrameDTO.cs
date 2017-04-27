namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of pictoframes when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class PictoFrameDTO : FrameDTO
    {
        /// <summary>
        /// The title of the pictoframe.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The accesslevel of the pictoframe.
        /// </summary>
        public AccessLevel AccessLevel { get; set; }

        /// <summary>
        /// Creates a new data transfer object for a given pictoframe.
        /// </summary>
        /// <param name="pictoframe">The pictoframe to create a DTO for.</param>
        public PictoFrameDTO(PictoFrame pictoframe) : base(pictoframe)
        {
            this.Title = pictoframe.Title;
            this.AccessLevel = pictoframe.AccessLevel;
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public PictoFrameDTO () {}
    }
}