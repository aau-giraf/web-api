using GirafRest.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    public class WeekTemplate : WeekBase
    {
        /// <summary>
        /// Empty constructor required for unit testing.
        /// </summary>
        public WeekTemplate()
        {
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public WeekTemplate(string name, Pictogram thumbnail, Department department) : base(thumbnail)
        {
            Name = name;
            DepartmentKey = department?.Key ?? -1;
        }

        [ForeignKey("Department")]
        public long DepartmentKey { get; set; }
        
        /// <summary>
        /// A reference to the department using this template.
        /// </summary>
        public virtual Department Department { get; set; }
    }
}
