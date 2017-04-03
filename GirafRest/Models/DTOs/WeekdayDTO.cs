using System.Collections;
using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    public class WeekdayDTO : PictoFrameDTO
    {
        public long ThumbnailID { get; set; }
        public bool ElementsSet { get; set; }
        public long[] ElementIDs { get; set; }
        public Days Day { get; set; }
        public ICollection<FrameDTO> Elements { get; set; }
        public WeekdayDTO(Weekday weekday) : base(weekday) {
            this.ThumbnailID = weekday.ThumbnailId;
            this.Day = weekday.Day;
            Elements = new List<FrameDTO>();
            foreach (var element in weekday.Elements)
            {
                Elements.Add(new FrameDTO(element.Resource));
            }
            if(Elements.Count > 0)
                ElementsSet = true;
        }
    }
}