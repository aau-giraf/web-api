using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models {
    /// <summary>
    /// A pictogram is an image with an associated title. They are used by Guardians and Citizens alike to 
    /// communicate visually.
    /// </summary>
    public class Pictogram : PictoFrame {
        /// <summary>
        /// A byte array containing the pictogram's image.
        /// </summary>
        [Column("Image")]
        public byte[] Image { get; set; }

        /// <summary>
        /// Creates a new pictogram with the given title and access level.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="accessLevel"></param>
        public Pictogram(string title, AccessLevel accessLevel) 
            : base(title, accessLevel)
        {
            
        }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Pictogram()
        {
            
        }
    }
}