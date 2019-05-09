namespace GirafRest.Models.DTOs
{
    public class ActivityDTO
    {
        public ActivityDTO(long id, WeekPictogramDTO pictogram, int order, ActivityState state)
        {
            this.Id = id;
            this.Pictogram = pictogram;
            this.Order = order;
            this.State = state;
        }

        public ActivityDTO(Activity weekdayResource)
        {
            this.Id = weekdayResource.Key;
            this.Pictogram = new WeekPictogramDTO(weekdayResource.Pictogram);
            this.Order = weekdayResource.Order;
            this.State = weekdayResource.State;
        }

        public ActivityDTO(Activity weekdayResource, WeekPictogramDTO pictogram)
        {
            this.Id = weekdayResource.Key;
            this.Order = weekdayResource.Order;
            this.State = weekdayResource.State;
            this.Pictogram = pictogram;
            this.Timer = new TimerDTO(weekdayResource.Timer);
        }

        public ActivityDTO(){}

        public WeekPictogramDTO Pictogram { get; set; }

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
