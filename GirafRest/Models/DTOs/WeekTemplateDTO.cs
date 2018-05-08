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

        public long DepartmentKey { get; internal set; }
        
        public long Id { get; set; }
    }
}
