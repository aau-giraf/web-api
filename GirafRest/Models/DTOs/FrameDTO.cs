using System;

namespace GirafRest.Models.DTOs
{
    public class FrameDTO
    {
        public long Id { get; set; }
        public DateTime LastEdit { get; set; }

        public FrameDTO (Frame frame) {
            this.Id = frame.Id;
            this.LastEdit = frame.LastEdit;
        }

        public FrameDTO () {}
    }
}