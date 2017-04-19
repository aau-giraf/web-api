using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models.DTOs
{
    public class DepartmentDTO
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public ICollection<string> Members { get; set; }
        public ICollection<long> Pictograms { get; set; }

        public DepartmentDTO(Department department)
        {
            this.ID = department.Key;
            this.Name = department.Name;
            this.Members = new List<string> (department.Members.Select(m => m.UserName));
            this.Pictograms = new List<long> (department.Resources.Select(dr => dr.ResourceKey));
        }

        public DepartmentDTO (long depID, string depName, 
            ICollection<string> depMembers, ICollection<long> depPictograms)
        {
            this.ID = depID;
            this.Name = depName;
            this.Members = depMembers;
            this.Pictograms = depPictograms;
        }
    }
}