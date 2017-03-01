using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafWebApi
    {
    public class Pictogram : PictoFrame {
        public GirafImage Image { get; set; }
        //this is a comment
            public Pictogram(string title, AccessLevel accessLevel, GirafImage image, string username) : base(title, accessLevel, username)
        {
            this.Image = image;
        }
        public Pictogram(string title, AccessLevel accessLevel, GirafImage image) : base(title, accessLevel)
        {
            this.Image = image;
        }
        public Pictogram(string title, AccessLevel accessLevel, GirafImage image, long department_key) : base(title, accessLevel, department_key)
        {
            this.Image = image;
        }
        public Pictogram(string title, AccessLevel accessLevel, GirafImage image, long department_key, string username)
            : base(title, accessLevel, department_key, username)
        {
            this.Image = image;
        }
        protected Pictogram(){ }

    }
}