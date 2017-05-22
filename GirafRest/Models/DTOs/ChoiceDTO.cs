using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of Choice when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class ChoiceDTO : ResourceDTO
    {
        /// <summary>
        /// A list of options that make up the choice.
        /// </summary>
        public List<PictogramDTO> Options;

        /// <summary>
        /// Creates a new ChoiceDTO from a given Choice.
        /// </summary>
        /// <param name="choice">The Choice to create the ChoiceDTO from.</param>
        public ChoiceDTO(Choice choice) :base(choice)
        {
            this.Title = choice.Title;
            this.Id = choice.Id;
            this.LastEdit = choice.LastEdit;
            Options = new List<PictogramDTO>();
            foreach (Pictogram p in choice)
            {
                this.Options.Add(new PictogramDTO(p));
            }
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public ChoiceDTO()
        {
            this.Title = "";
        }
    }
}
