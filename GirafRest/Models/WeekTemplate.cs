using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    public class WeekTemplate
    {
        [ForeignKey("Department")]
        public long DepartmentKey { get; set; }
        /// <summary>
        /// A reference to the department using this template.
        /// </summary>
        public virtual Department Department { get; set; }

        public Week Week { get; set; }
    }
}
