using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public class UserResource : IManyToMany<string, GirafUser>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}

        [Required]
        public string OtherKey { get; set; }
        [ForeignKey("OtherKey")]
        public virtual GirafUser Other {get; set;}

        [Required]
        public long ResourceKey { get; set; }
        [ForeignKey("ResourceKey")]
        public virtual Frame Resource { get; set; }

        public UserResource(GirafUser user, Frame resource)
        {
            this.OtherKey = user.Id;
            this.Other = user;
            this.ResourceKey = resource.Key;
            this.Resource = resource;

            Other.Resources.Add(this);
            Resource.Users.Add(this);
        }

        public UserResource(){ }
    }
}