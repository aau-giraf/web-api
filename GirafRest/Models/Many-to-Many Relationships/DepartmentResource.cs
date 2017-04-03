using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public class DepartmentResource : IManyToMany<long, Department>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}

        [Required]
        public long OtherKey { get; set; }
        [ForeignKey("OtherKey")]
        public virtual Department Other {get; set;}

        [Required]
        public long ResourceKey { get; set; }
        [ForeignKey("ResourceKey")]
        public virtual Frame Resource { get; set; }

        public DepartmentResource(Department dep, Frame resource)
        {
            this.OtherKey = dep.Key;
            this.Other = dep;
            this.ResourceKey = resource.Id;
            this.Resource = resource;

            Other.Resources.Add(this);
            resource.Departments.Add(this);
        }

        public DepartmentResource(){}
    }
}