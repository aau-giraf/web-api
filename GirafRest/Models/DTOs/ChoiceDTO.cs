using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    public class ChoiceDTO : FrameDTO
    {
        public List<PictoFrame> Options;

        public ChoiceDTO(Choice choice)
        {
            this.Id = choice.Key;
            this.LastEdit = choice.LastEdit;
            foreach (PictoFrame p in choice)
            {
                this.Options.Add(p);
            }
        }
    }
}
