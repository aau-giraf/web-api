using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models {
    public class Pictogram : PictoFrame {
        public Pictogram(string title, AccessLevel accessLevel) 
            : base(title, accessLevel)
        {
            
        }

        public Pictogram()
        {
            
        }
    }
}