using System.Collections.Generic;

namespace GirafWebApi.Models.DTOs
{
    public class SequenceDTO : PictoFrameDTO
    {
        public long ThumbnailID { get; set; }
        public bool ElementsSet { get; set; }
        public long[] ElementIDs { get; set; }
    }
}