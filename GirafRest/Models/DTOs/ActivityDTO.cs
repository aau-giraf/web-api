namespace GirafRest.Models.DTOs
{
    public class ActivityDTO
    {
        public ActivityDTO(WeekPictogramDTO pictogram, int order, ActivityState state)
        {
            this.Id = pictogram.Id;
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

        public ActivityDTO(){}

        public WeekPictogramDTO Pictogram { get; set; }

        public int Order { get; set; }
        
        public ActivityState State { get; set; }

        public long Id { get; internal set; }

        /// <summary>
        /// This is used in the WeekPlanner app by the frontend groups and should never be set from our side
        /// </summary>
        public bool IsChoiceBoard { get; set; }
    }
}
