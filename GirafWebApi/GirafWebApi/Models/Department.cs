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

        public ICollection<Pictogram> Pictograms { get; set; }

        public Department () {
            Members = new List<GirafUser>();
            Pictograms = new List<Pictogram>();
        }
    }
}