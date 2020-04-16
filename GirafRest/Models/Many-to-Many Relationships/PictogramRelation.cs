using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models;

namespace GirafRest
{
    public class PictogramRelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string ActivityId { get; set; }

        [Required]
        public string PictogramId { get; set; }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        [ForeignKey("PictogramId")]
        public virtual Pictogram Pictogram { get; set; }

        public PictogramRelation(Activity activity, Pictogram pictogram)
        {
            this.Activity = activity;
            this.Pictogram = pictogram;
        }

        public PictogramRelation()
        {

        }
    }

}
