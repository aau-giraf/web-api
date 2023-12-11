using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafEntities.WeekPlanner;


namespace GirafEntities.WeekPlanner
{
    public class PictogramRelation
    {

        [Required]
        public long ActivityId { get; set; }

        [Required]
        public long PictogramId { get; set; }

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
