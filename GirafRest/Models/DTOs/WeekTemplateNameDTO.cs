using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    public class WeekTemplateNameDTO
    {
        /// <summary>
        /// Name of the <see cref="WeekTemplate"/>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of the <see cref="WeekTemplate"/>
        /// </summary>
        public long TemplateId { get; set; }

        public WeekTemplateNameDTO()
        {
        }

        public WeekTemplateNameDTO(string name, long templateID)
        {
            this.Name = name;
            this.TemplateId = templateID;
        }
    }
}
