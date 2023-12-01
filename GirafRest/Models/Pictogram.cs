using GirafRest.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    /// <summary>
    /// A pictogram is an image with an associated title. They are used by Guardians and Citizens and so on to 
    /// communicate visually.
    /// </summary>
    public class Pictogram
    {
        /// <summary>
        /// Pictogram title/name
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Primary key; Identity autoincrementing
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Last edited at
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime LastEdit { get; set; }

        /// <summary>
        /// Belonging Users
        /// </summary>
        public ICollection<UserResource> Users { get; set; }

        /// <summary>
        /// Belonging departments
        /// </summary>
        public ICollection<DepartmentResource> Departments { get; set; }

        /// <summary>
        /// Getting activity relations
        /// </summary>
        public ICollection<PictogramRelation> Activities { get; set;  }

        /// <summary>
        /// AccessLevel managing
        /// </summary>
        [Required]
        public AccessLevel AccessLevel { get; set; }

        /// <summary>
        /// Hash of the image
        /// </summary>
        [Column("ImageHash")]
        public string ImageHash { get; set; }
        
        /// <summary>
        /// A list of alternate names of the pictogram
        /// </summary>
        public ICollection<AlternateName> AlternateNames { get; set; }

        /// <summary>
        /// Currently not used, but old applications needs this
        /// </summary>
        public byte[] Sound { get; set; }

        /// <summary>
        /// Empty constructor is required by Newtonsoft.
        /// </summary>
        public Pictogram()
        {
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
            if (other.ImageHash != null)
                this.ImageHash = other.ImageHash;
        }

        /// <summary>
        /// DO NOT DELETE
        /// </summary>
        public Pictogram(string title, AccessLevel accessLevel) : this()
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Image title</param>
        /// <param name="accessLevel">required accesslevel</param>
        /// <param name="imageHash">hash of image</param>
        public Pictogram(string title, AccessLevel accessLevel, string imageHash) : this()
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
            this.ImageHash = imageHash;
        }
    }
}