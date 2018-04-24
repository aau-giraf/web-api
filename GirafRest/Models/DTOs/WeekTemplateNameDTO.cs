using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    public class WeekTemplateNameDTO
    {
        public string Name { get; set; }

        public WeekTemplateNameDTO()
        {
        }

        public WeekTemplateNameDTO(string name)
        {
            this.Name = name;
        }
    }
}
