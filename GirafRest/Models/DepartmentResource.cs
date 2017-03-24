using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public class DepartmentResource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}


        [Required]
        public long DeparmentKey { get; set; }
        [ForeignKey("DeparmentKey")]
        public virtual Department Department {get; set;}

        [Required]
        public long PictoFrameKey { get; set; }
        [ForeignKey("PictoFrameKey")]
        public virtual PictoFrame Resource { get; set; }

        public DepartmentResource(Department dep, PictoFrame resource)
        {
            this.DeparmentKey = dep.Key;
            this.Department = dep;
            this.PictoFrameKey = resource.Key;
            this.Resource = resource;

            dep.Resources.Add(this);
            resource.Departments.Add(this);
        }

        public DepartmentResource(){}
    }
}