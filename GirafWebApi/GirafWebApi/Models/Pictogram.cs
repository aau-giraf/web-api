using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafWebApi.Models {
    public class Pictogram : PictoFrame {
        public GirafImage Image { get; set; }
        
        public Pictogram(string title, AccessLevel accessLevel, GirafImage image, GirafUser user) 
            : base(title, accessLevel, user)
        {
            this.Image = image;
        }
        public Pictogram(string title, AccessLevel accessLevel, GirafImage image) 
            : base(title, accessLevel)
        {
            this.Image = image;
        }
        public Pictogram(string title, AccessLevel accessLevel, GirafImage image, long department_key) 
            : base(title, accessLevel, department_key)
        {
            this.Image = image;
        }
        public Pictogram(string title, AccessLevel accessLevel, GirafImage image, long department_key, string user_id)
            : base(title, accessLevel, department_key, user_id)
        {
            this.Image = image;
        }
        protected Pictogram(){ }
    }
}