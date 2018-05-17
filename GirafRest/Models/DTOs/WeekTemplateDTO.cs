using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    public class WeekTemplateDTO : WeekBaseDTO
    {
        public WeekTemplateDTO() : base()
        {

        }

        public WeekTemplateDTO(WeekTemplate weekTemplate) : base(weekTemplate)
        {
            this.DepartmentKey = weekTemplate.DepartmentKey;
            this.Id = weekTemplate.Id;
        }

        /// <summary>
        /// Key of the department the template belongs to
        /// </summary>
        public long DepartmentKey { get; internal set; }

        /// <summary>
        /// Id of the corresponding <see cref="WeekTemplate"/>
        /// </summary>
        public long Id { get; set; }
    }
}
