using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models {
    /// <summary>
    /// PictoFrame further specializes the Resource class and defines common attributes for all resources that
    /// may be part of a Weekday.
    /// </summary>
    public abstract class PictoFrame : Resource{
        /// <summary>
        /// The title of the PictoFrame.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The accesslevel, PRIVATE means owning user only, PROTECTED means all in the owning department and PUBLIC is everyone.
        /// </summary>
        [Required]
        public AccessLevel AccessLevel { get; set; }

        /// <summary>
        /// Creates a new PictoFrame.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="accessLevel">The access level.</param>
        public PictoFrame(string title, AccessLevel accessLevel) : this()
        {
            this.Title = title;
            this.AccessLevel = accessLevel;
        }
        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        protected PictoFrame() : base() {
            AccessLevel = AccessLevel.PUBLIC;
        }

        /// <summary>
        /// Overrides the information of this PictoFrame with new information found in the DTO.
        /// </summary>
        /// <param name="other">The new information.</param>
        public virtual void Merge(PictoFrameDTO other) {
            base.Merge(other);
            this.AccessLevel = other.AccessLevel;
        }
    }
}