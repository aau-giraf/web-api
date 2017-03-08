using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafWebApi.Models {
    public abstract class PictoFrame : Frame {
        [Required]
        protected string Title { get; set; }

        public string owner_id { get; set; }
        [ForeignKey("owner_id")]
        protected GirafUser owner { get; set; }

        [Required]
        protected AccessLevel AccessLevel { get; set; }

        public long Department_Key { get; set; }
        [ForeignKey("Department_Key")]
        public Department Department { get; set; }

        public PictoFrame(string title, AccessLevel accessLevel)
        {
            this.Title = title;

            this.AccessLevel = accessLevel;
        }

        public PictoFrame(string title, AccessLevel accessLevel, long department_key) : this(title, accessLevel)
        {
            this.Department_Key = department_key;
        }


        public PictoFrame(string title, AccessLevel accessLevel, long department_key, string user_id)
        : this(title, accessLevel, department_key)
        {
            this.owner_id = user_id;
        }

        public PictoFrame(string title, AccessLevel accessLevel, GirafUser user)
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
            this.owner_id = user.Id; 
            this.Department_Key = user.Department_Key;
        }
        protected PictoFrame() {}

        public virtual void Merge(PictoFrame other) {
            this.owner_id = other.owner_id;
            this.Department_Key = other.Department_Key;
            this.AccessLevel = other.AccessLevel;
        }
    }
}