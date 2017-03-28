using System.Collections;
using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    public class SequenceDTO : PictoFrameDTO
    {
        public long ThumbnailID { get; set; }
        public bool ElementsSet { get; set; }
        public long[] ElementIDs { get; set; }
        public ICollection<Frame> Elements { get; set; }
        public SequenceDTO(Sequence sequence){
            this.AccessLevel = sequence.AccessLevel;
            this.Id = sequence.Key;
            this.Title = sequence.Title;
            this.LastEdit = sequence.LastEdit;
            this.ThumbnailID = sequence.ThumbnailKey;
            foreach (var element in sequence.GetAll())
            {
                Elements.Add(element);
            }
            if(Elements != null)
                ElementsSet = true;
        }
    }
}