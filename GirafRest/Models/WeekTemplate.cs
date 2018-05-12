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
        [ForeignKey("Department")]
        public long DepartmentKey { get; set; }
        
        /// <summary>
        /// A reference to the department using this template.
        /// </summary>
        public virtual Department Department { get; set; }

        /// <summary>
        /// DO NOT DELETE
        /// </summary>
        public WeekTemplate()
        {
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public WeekTemplate(Department department)
        {
            DepartmentKey = department?.Key ?? -1;
        }
    }
}
