using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models {
    public abstract class PictoFrame : Frame{
        public string Title { get; set; }
        [Required]
        public AccessLevel AccessLevel { get; set; }

        public PictoFrame(string title, AccessLevel accessLevel) : this()
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
        }
        protected PictoFrame() : base() {
            AccessLevel = AccessLevel.PUBLIC;
        }

        public virtual void Merge(PictoFrameDTO other) {
            base.Merge(other);
            this.AccessLevel = other.AccessLevel;
        }
    }
}