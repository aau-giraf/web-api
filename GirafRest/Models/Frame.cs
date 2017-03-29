using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models {
    public abstract class Frame {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime LastEdit { get; set; }

        //public ICollection<WeekdayResource> PartOfSequences { get; set; }

        public ICollection<UserResource> Users { get; set; }
        public ICollection<DepartmentResource> Departments { get; set; }

        public virtual void Merge(FrameDTO other) {
            if(other.Id != this.Key) throw new ArgumentException("Two pictograms with different IDs may not be merged.");
            this.LastEdit = DateTime.Now;
        }

        public Frame()
        {
            //PartOfSequences = new List<WeekdayResource>();
            Users = new List<UserResource>();
            Departments = new List<DepartmentResource>();
            LastEdit = DateTime.Now;
        }
    }
}