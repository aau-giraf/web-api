using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models {
    /// <summary>
    /// A pictogram is an image with an associated title. They are used by Guardians and Citizens and so on to 
    /// communicate visually.
    /// </summary>
    public class Pictogram{

        [Required]
        public string Title { get; set; }

        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public System.DateTime LastEdit { get; set; }

        public ICollection<UserResource> Users { get; set; }

        public ICollection<DepartmentResource> Departments { get; set; }

        [Required]
        public AccessLevel AccessLevel { get; set; }

        [Column("ImageHash")]
        public String ImageHash { get; set; }

        /// <summary>
        /// Currently not used, but old applications needs this
        /// </summary>
        public byte[] Sound { get; set; }

        /// <summary>
        /// Empty constructor is required by Newtonsoft.
        /// </summary>
        public Pictogram(){
            Users = new List<UserResource>();
            Departments = new List<DepartmentResource>();
            LastEdit = DateTime.Now;
        }

        /// <summary>
        /// Overrides the information of this Pictogram with new information found in the given DTO.
        /// </summary>
        /// <param name="other">The new information.</param>
        public virtual void Merge(PictogramDTO other)
        {
            this.LastEdit = DateTime.Now;
            this.AccessLevel = (AccessLevel)other.AccessLevel;
            this.Title = other.Title;
            if(other.ImageHash != null)
                this.ImageHash = other.ImageHash;
        }

        // DO NOT DELETE
        public Pictogram(string title, AccessLevel accessLevel) : this()
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
        }

        public Pictogram(string title, AccessLevel accessLevel, String imageHash) : this()
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
            this.ImageHash = imageHash;
        }
    }
}