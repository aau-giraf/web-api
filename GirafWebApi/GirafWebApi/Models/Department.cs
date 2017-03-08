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

        public ICollection<GirafUser> members { get; set; }

        public ICollection<Pictogram> pictograms { get; set; }

        public Department () {
            members = new List<GirafUser>();
            pictograms = new List<Pictogram>();
        }
    }
}