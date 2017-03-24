using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public class Department {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<GirafUser> Members { get; set; }
        public virtual ICollection<DepartmentResource> Resources { get; set; }

        public Department () {
            Members = new List<GirafUser>();
            Resources = new List<DepartmentResource>();
        }
    }
}