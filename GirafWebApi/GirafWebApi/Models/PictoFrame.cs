using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafWebApi
{
    public abstract class PictoFrame : Frame 
    {
        protected string Title { get; set; }

        public string owner_name { get; set; }
        [ForeignKey("owner_name")]
        protected GirafUser owner { get; set; }

        protected AccessLevel accessLevel { get; set; }

        public long Department_Key { get; set; }
        [ForeignKey("Department_Key")]
        public Department Department { get; set; }

        public PictoFrame(string title, AccessLevel accessLevel)
        {
            this.Title = title;
            this.accessLevel = accessLevel;
        }

        public PictoFrame(string title, AccessLevel accessLevel, long department_key) : this(title, accessLevel)
        {
            this.Department_Key = department_key;
        }

        public PictoFrame(string title, AccessLevel accessLevel, long department_key, string username)
        : this(title, accessLevel, department_key)
        {
            this.owner_name = username;
        }

        public PictoFrame(string title, AccessLevel accessLevel, string username)
        {
            this.Title = title;
            this.accessLevel = accessLevel;
            this.owner_name = username;
            //Todo: find and fill deparment_key
        }
        protected PictoFrame() 
        {
            
        }
    }
}