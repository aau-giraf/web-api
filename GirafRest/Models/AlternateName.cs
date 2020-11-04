using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    [Table("AlternateNames")]
    public class AlternateName
    {
        /// <summary>
        /// The unique identifier for the object
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("CitizenId")] 
        public string CitizenId { get; set; }
        public GirafUser Citizen { get; }
        
        [ForeignKey("PictogramId")]
        public long PictogramId { get; set; }
        public Pictogram Pictogram { get; }

        [Column("Name")]
        public string Name { get; set; }

        AlternateName(GirafUser citizen, Pictogram pictogram, string name)
        {
            Citizen = citizen;
            Pictogram = pictogram;
            Name = name;
        }
        
    }
}