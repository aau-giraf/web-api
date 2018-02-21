using GirafRest.Models.DTOs;
using GirafRest.Models.Many_to_Many_Relationships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace GirafRest.Models {
    /// <summary>
    /// A Choice may be present in the Citizens Weekdays and indicates that the user may choose between a
    /// series of activities.
    /// </summary>
    public class Choice : Resource { 
        /// <summary>
        /// A list of options involved in the choice.
        /// </summary>
        public ICollection<ChoiceResource> Options { get; private set; }

        /// <summary>
        /// Empty constructor is required by Newtonsoft.
        /// </summary>
        public Choice () {}

        /// <summary>
        /// Creates a new Choice with the given list of options.
        /// </summary>
        /// <param name="options">A list of options to add to the choice.</param>
        public Choice(List<Pictogram> options, string title)
        {
            this.Title = title;
            LastEdit = DateTime.Now;
            Options = new List<ChoiceResource>();
            foreach (Pictogram option in options)
            {
                Options.Add(new ChoiceResource(this, option));
            }
        }

        /// <summary>
        /// Adds an option to the choice.
        /// </summary>
        /// <param name="option">The pictogram to be added as an option.</param>
        public void Add (Pictogram option) => Options.Add(new ChoiceResource(this, option));
        /// <summary>
        /// Adds all the options from the given list.
        /// </summary>
        /// <param name="options">A list of options to add to the choice.</param>
        public void AddAll(ICollection<Pictogram> options) {
            foreach (var option in options) {
                Options.Add(new ChoiceResource(this, option));
            }
        }

        /// <summary>
        /// Get the index'th option of the Choice.
        /// </summary>
        /// <param name="index">The index to fetch the pictogram from.</param>
        /// <returns>The Pictogram of the index'th slot.</returns>
        public Pictogram Get(int index) => (Pictogram) Options.ElementAt(index).Resource;

        /// <summary>
        /// Remove the given pictogram from the list of options.
        /// </summary>
        /// <param name="pictogram">The pictogram to remove.</param>
        public void Remove(Pictogram pictogram) => Options.Remove(new ChoiceResource(this, pictogram));

        /// <summary>
        /// Clears all options of the choice.
        /// </summary>
        public void Clear() => Options.Clear();

        /// <summary>
        /// Gets the enumerator of the Choice.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Pictogram> GetEnumerator()
        {
            List<Pictogram> pictogramList = new List<Pictogram>();
            foreach (var choiceResource in Options)
            {
                pictogramList.Add((Pictogram) choiceResource.Resource);
            }
            return pictogramList.GetEnumerator();
        }
    }
}