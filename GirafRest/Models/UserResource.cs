using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public class UserResource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual GirafUser User {get; set;}

        [Required]
        public long PictoFrameKey { get; set; }
        [ForeignKey("PictoFrameKey")]
        public virtual PictoFrame Resource { get; set; }

        public UserResource(GirafUser user, PictoFrame resource)
        {
            this.UserId = user.Id;
            this.User = user;
            this.PictoFrameKey = resource.Key;
            this.Resource = resource;

            User.Resources.Add(this);
            Resource.Users.Add(this);
        }

        public UserResource(){ }
    }
}