using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models {
    /// <summary>
    /// A pictogram is an image with an associated title. They are used by Guardians and Citizens and so on to 
    /// communicate visually.
    /// </summary>
    public class Pictogram : Resource{
        
        /// <summary>
        /// The accesslevel, PRIVATE means only the owner can see it, PROTECTED means everyone in the owning department and PUBLIC is everyone.
        /// </summary>
        [Required]
        public AccessLevel AccessLevel { get; set; }
        
        /// <summary>
        /// A byte array containing the pictogram's image.
        /// </summary>
        [Column("Image")]
        public byte[] Image { get; set; }

        /// <summary>
        /// Currently not used, but the SymmetricDS pictograms contained this field.
        /// </summary>
        public byte[] Sound { get; set; }

        /// <summary>
        /// Creates a new pictogram with the given title and access level.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="accessLevel"></param>
        public Pictogram(string title, AccessLevel accessLevel)
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
        }

        /// <summary>
        /// Creates a new pictogram with the given title, access level and image.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="accessLevel"></param>
        /// <param name="image"></param>
        public Pictogram(string title, AccessLevel accessLevel, byte[] image)
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
            this.Image = image;
        }

        /// <summary>
        /// Empty constructor is required by Newtonsoft.
        /// </summary>
        public Pictogram(){}

        /// <summary>
        /// Overrides the information of this Pictogram with new information found in the given DTO.
        /// </summary>
        /// <param name="other">The new information.</param>
        public virtual void Merge(PictogramDTO other)
        {
            base.Merge(other);
            this.AccessLevel = (AccessLevel)other.AccessLevel;
            this.Title = other.Title;
            if(other.Image != null)
                this.Image = other.Image;
        }
    }
}