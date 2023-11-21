using System;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="Pictogram"/>
    /// </summary>
    public class WeekPictogramDTO
    {
        /// <summary>
        /// Primary key 
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
        /// Hash of image
        /// </summary>
        public string ImageHash { get; set; }

        /// <summary>
        /// Generated URL from ID
        /// </summary>
        public string ImageUrl { get { return $"/v1/pictogram/{Id}/image/raw"; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pictogram">Pictogram used as base</param>
        public WeekPictogramDTO(Pictogram pictogram)
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
        public WeekPictogramDTO()
        {
        }
    }
}
