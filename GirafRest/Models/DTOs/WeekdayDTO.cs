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
        public ICollection<FrameDTO> _elements { get; set; }
        public WeekdayDTO(Weekday weekday){
            this.AccessLevel = weekday.AccessLevel;
            this.Id = weekday.Key;
            this.Title = weekday.Title;
            this.LastEdit = weekday.LastEdit;
            this.Day = weekday.Day;
            _elements = new List<FrameDTO>();
            foreach (var element in weekday._elements)
            {
                _elements.Add(new PictogramDTO(new Pictogram()) { Id = element.Key, LastEdit = element.LastEdit});
                //_elements.Add(new FrameDTO() { Id = element.Key, LastEdit = element.LastEdit});
            }
            if(_elements != null)
                ElementsSet = true;
        }
    }
}