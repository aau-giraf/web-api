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
        public WeekTemplate()
        {
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public WeekTemplate(Pictogram thumbnail) : base(thumbnail)
        {
        }
        /// <summary>
        /// Creates a new WeekTemplate from the given WeekDTO.
        /// </summary>
        /// <param name="weekDTO">The data transfer object to create a new week template from.</param>
        public WeekTemplate(WeekDTO weekDTO, long departmentKet) : base(weekDTO)
        {
        }

        [ForeignKey("Department")]
        public long DepartmentKey { get; set; }
        /// <summary>
        /// A reference to the department using this template.
        /// </summary>
        public virtual Department Department { get; set; }
    }
}
