using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models {
    /// <summary>
    /// Resource is the base class of all classes used by the users, e.g. pictograms and choices.
    /// </summary>
    public abstract class Resource {
        /// <summary>
        /// The id of the resource.
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// The last time the given resource was edited.
        /// </summary>
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime LastEdit { get; set; }

        /// <summary>
        /// A collection of all users who owns the resource.
        /// </summary>
        public ICollection<UserResource> Users { get; set; }
        /// <summary>
        /// A collection of all departments who owns the resource.
        /// </summary>
        public ICollection<DepartmentResource> Departments { get; set; }

        /// <summary>
        /// Overrides the data of the current instance with the information found in 'other'.
        /// </summary>
        /// <param name="other">The data to override with.</param>
        public virtual void Merge(FrameDTO other) {
            if(other.Id != this.Id) throw new ArgumentException("Two pictograms with different IDs may not be merged.");
            this.LastEdit = DateTime.Now;
        }

        /// <summary>
        /// Creates a new resource.
        /// </summary>
        public Resource()
        {
            Users = new List<UserResource>();
            Departments = new List<DepartmentResource>();
            LastEdit = DateTime.Now;
        }
    }
}