namespace GirafRest.Models.DTOs
{
    public class ActivityDTO
    {
        public ActivityDTO(WeekPictogramDTO pictogram, int order)
        {
            this.Id = pictogram.Id;
            this.Pictogram = pictogram;
            this.Order = order;
        }

        public ActivityDTO(Activity weekdayResource)
        {
            this.Id = weekdayResource.Key;
            this.Pictogram = new WeekPictogramDTO(weekdayResource.Pictogram);
            this.Order = weekdayResource.Order;
        }

        public ActivityDTO(){}

        public WeekPictogramDTO Pictogram { get; set; }

        public int Order { get; set; }

        public long Id { get; internal set; }
    }
}
