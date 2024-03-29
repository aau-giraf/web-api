using System;

namespace GirafEntities.WeekPlanner.DTOs
{
    /// <summary>
    /// Defines the structure of Pictogram when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class PictogramDTO
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The last time the pictogram was edited.
        /// </summary>
        public DateTime LastEdit { get; set; }

        /// <summary>
        /// The title of the pictogram.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The accesslevel of the pictogram.
        /// </summary>
        public AccessLevel? AccessLevel { get; set; }

        /// <summary>
        /// Hash of DTO
        /// </summary>
        public string? ImageHash { get; set; }

        /// <summary>
        /// Generated image URL
        /// </summary>
        public string ImageUrl { get { return $"/v1/pictogram/{Id}/image/raw"; } }

        /// <summary>
        /// Construcotr given Pictogram
        /// </summary>
        /// <param name="pictogram">Pictogram to base DTO of</param>
        public PictogramDTO(Pictogram pictogram)
        {
            if (pictogram != null)
            {
                this.Title = pictogram.Title;
                this.AccessLevel = pictogram.AccessLevel;
                this.Id = pictogram.Id;
                this.LastEdit = pictogram.LastEdit.ToUniversalTime();
                this.ImageHash = pictogram.ImageHash;
            }
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public PictogramDTO()
        {
        }
    }
}