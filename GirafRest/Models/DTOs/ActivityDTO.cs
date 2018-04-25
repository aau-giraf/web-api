namespace GirafRest.Models.DTOs
{
    public class ActivityDTO
    {
        public ActivityDTO(WeekdayResource weekdayResource)
        {
            this.Id = weekdayResource.Key;
            this.Pictogram = new PictogramDTO(weekdayResource.Pictogram);
            this.Order = weekdayResource.Order;
        }

        public ActivityDTO(){}

        public PictogramDTO Pictogram { get; set; }

        public int Order { get; set; }

        public long Id { get; set; }
    }
}
