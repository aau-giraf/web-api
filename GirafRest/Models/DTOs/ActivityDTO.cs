namespace GirafRest.Models.DTOs
{
    public class ActivityDTO
    {
        public ActivityDTO(PictogramDTO pictogram, int order)
        {
            this.Pictogram = pictogram;
            this.Order = order;
        }

        public ActivityDTO(WeekdayResource weekdayResource)
        {
            this.Pictogram = new PictogramDTO(weekdayResource.Pictogram);
            this.Order = weekdayResource.Order;
        }

        public ActivityDTO(){}

        public PictogramDTO Pictogram { get; set; }

        public int Order { get; set; }
    }
}
