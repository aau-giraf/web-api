using System.Collections.Generic;

namespace GirafWebApi.Models.DTOs
{
    public class SequenceDTO : PictoFrameDTO
    {
        public ICollection<Frame> _elements {get; set;}
        public Pictogram Thumbnail { get; set; }
        
    }
}