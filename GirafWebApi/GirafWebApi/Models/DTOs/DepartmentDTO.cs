using System.Collections.Generic;

namespace GirafWebApi.Models.DTOs
{
    public class DepartmentDTO
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public ICollection<GirafUser> members { get; set; }
        public ICollection<Pictogram> pictograms { get; set; }

    }
}