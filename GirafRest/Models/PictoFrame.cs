using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models {
    public abstract class PictoFrame : Frame {
        [Required]
        public string Title { get; set; }

        [Required]
        public AccessLevel AccessLevel { get; set; }

        public ICollection<UserResource> Users { get; set; }
        public ICollection<DepartmentResource> Departments { get; set; }

        public PictoFrame(string title, AccessLevel accessLevel) : this()
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
        }
        protected PictoFrame() {
            this.Users = new List<UserResource>();
            this.Departments = new List<DepartmentResource>();
        }

        public virtual void Merge(PictoFrameDTO other) {
            base.Merge(other);
            this.AccessLevel = other.AccessLevel;
        }
    }
}