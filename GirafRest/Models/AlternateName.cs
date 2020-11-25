using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    
    /// <summary>
    /// Class for saving alternate names for pictograms 
    /// </summary>
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
        public GirafUser Citizen { get; set; }
        
        [ForeignKey("PictogramId")]
        public long PictogramId { get; set; }
        public Pictogram Pictogram { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        public AlternateName()
        {
            
        }
        
        public AlternateName(GirafUser citizen, Pictogram pictogram, string name)
        {
            Citizen = citizen;
            CitizenId = Citizen.Id;
            Pictogram = pictogram;
            PictogramId = Pictogram.Id;
            Name = name;
        }

    }
}