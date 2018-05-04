using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    public class WeekPictogramDTO
    {
        public long Id { get; set; }
        /// <summary>
        /// The last time the pictogram was edited.
        /// </summary>
        public DateTime LastEdit { get; internal set; }

        /// <summary>
        /// The title of the pictogram.
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// The accesslevel of the pictogram.
        /// </summary>
        public AccessLevel? AccessLevel { get; internal set; }

        /// <summary>
        /// An array of bytes containing the pictogram's image.
        /// </summary>
        [JsonIgnore]
        public byte[] Image { get; internal set; }
        public string ImageUrl { get { return $"/v1/pictogram/{Id}/image/raw"; } }
        public string ImageHash { get { return Image == null ? null : Convert.ToBase64String(MD5.Create().ComputeHash(Image)); } }

        public WeekPictogramDTO(Pictogram pictogram)
        {
            if (pictogram != null)
            {
                this.Title = pictogram.Title;
                this.AccessLevel = pictogram.AccessLevel;
                this.Id = pictogram.Id;
                this.LastEdit = pictogram.LastEdit;
                this.Image = pictogram.Image;
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
