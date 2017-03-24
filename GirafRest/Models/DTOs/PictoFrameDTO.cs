using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    public class PictoFrameDTO : FrameDTO
    {
        public string Title { get; set; }
        public ICollection<GirafUser> Users { get; set; }
        public ICollection<Department> Departments { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
}