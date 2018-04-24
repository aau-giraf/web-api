using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    public class PictogramUpdateDTO
    {
        [Required]
        /// <summary>
        /// The title of the pictogram.
        /// </summary>
        public string Title { get; set; }

        [Required]
        /// <summary>
        /// The accesslevel of the pictogram.
        /// </summary>
        public AccessLevel? AccessLevel { get; set; }

        

        public PictogramUpdateDTO() : base()
        {
        }

        /// <summary>
        /// Creates a new pictogram data transfer object from a given pictogram.
        /// </summary>
        /// <param name="Pictogram">The pictogram to create a DTO for.</param>
        public PictogramUpdateDTO(Pictogram pictogram)
        {
            if (pictogram != null)
            {
                this.Title = pictogram.Title;
                this.AccessLevel = pictogram.AccessLevel;
            }
        }

        
    }
}
