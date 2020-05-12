using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    /// <summary>
    /// Defines a many-to-many relationship between <see cref="GirafUser"/> and <see cref="Pictogram"/> (ressource)
    /// </summary>
    public class UserResource
    {
        /// <summary>
        /// The key of the relationship entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}

        /// <summary>
        /// The key of the user who is involved in the relationship.
        /// </summary>
        [Required]
        public string OtherKey { get; set; }
        /// <summary>
        /// A reference to the actual user.
        /// </summary>
        [ForeignKey("OtherKey")]
        public virtual GirafUser Other {get; set;}

        /// <summary>
        /// The key of the involved resource.
        /// </summary>
        [Required]
        public long PictogramKey { get; set; }

        /// <summary>
        /// A reference to the actual resource.
        /// </summary>
        [ForeignKey("PictogramKey")]
        public virtual Pictogram Pictogram { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a user and a resource.
        /// </summary>
        /// <param name="user">The involved user.</param>
        /// <param name="pictogram">The involved pictogram.</param>
        public UserResource(GirafUser user, Pictogram pictogram)
        {
            if (user == null) {
                throw new System.ArgumentNullException(user + " is null");
            } else if (pictogram == null) {
                throw new System.ArgumentNullException(pictogram + " is null");
            }
            this.OtherKey = user.Id;
            this.Other = user;
            this.PictogramKey = pictogram.Id;
            this.Pictogram = pictogram;

            pictogram.LastEdit = DateTime.Now;
            Other.Resources.Add(this);
            pictogram.Users.Add(this);
        }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public UserResource(){ }
    }
}