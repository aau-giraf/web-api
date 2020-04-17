using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    public class ActivityDTO
    {
        public ActivityDTO(long id, List<WeekPictogramDTO> pictograms, int order, ActivityState state)
        {
            this.Id = id;
            this.Pictograms = pictograms;
            this.Order = order;
            this.State = state;
        }

        public ActivityDTO(Activity weekdayResource)
        {
            this.Id = weekdayResource.Key;
            this.Order = weekdayResource.Order;
            this.State = weekdayResource.State;
            this.Pictograms = new List<WeekPictogramDTO>();

            foreach (var relation in weekdayResource.Pictograms)
            {
                this.Pictograms.Add(new WeekPictogramDTO(relation.Pictogram));
            }

            if (weekdayResource.Timer != null)
            {
                this.Timer = new TimerDTO(weekdayResource.Timer);
            }
        }

        public ActivityDTO(Activity weekdayResource, List<WeekPictogramDTO> pictograms)
        {
            this.Id = weekdayResource.Key;
            this.Order = weekdayResource.Order;
            this.State = weekdayResource.State;
            this.Pictograms = pictograms;
        }

        public ActivityDTO(){}

        public ICollection<WeekPictogramDTO> Pictograms { get; set; }

        /// <summary>
        /// The order that the activity will appear on in a weekschedule. If two has same order it is a choice
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The current ActivityState
        /// </summary>
        public ActivityState State { get; set; }

        public long Id { get; set; }

        /// <summary>
        /// This is used in the WeekPlanner app by the frontend groups and should never be set from our side
        /// </summary>
        public bool IsChoiceBoard { get; set; }

        public TimerDTO Timer { get; set; } 
    }
}
