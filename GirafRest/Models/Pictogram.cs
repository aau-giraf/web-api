using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models {
    public class Pictogram : PictoFrame {
        [Column("Image")]
        public byte[] Image { get; set; }
        public Pictogram(string title, AccessLevel accessLevel) 
            : base(title, accessLevel)
        {
            
        }

        public Pictogram()
        {
            
        }
    }
}