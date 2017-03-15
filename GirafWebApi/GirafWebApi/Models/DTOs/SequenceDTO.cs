using System.Collections.Generic;

namespace GirafWebApi.Models.DTOs
{
    public class SequenceDTO : PictoFrameDTO
    {
        public long thumbnail_id { get; set; }
        public bool elements_set { get; set; }
        public long[] element_ids { get; set; }
    }
}