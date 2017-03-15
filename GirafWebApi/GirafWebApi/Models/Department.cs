using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafWebApi.Models
{
    public class Department {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<GirafUser> Members { get; set; }

        public ICollection<PictoFrame> Resources { get; set; }

        public Department () {
            Members = new List<GirafUser>();
            Resources = new List<PictoFrame>();
        }
    }
}